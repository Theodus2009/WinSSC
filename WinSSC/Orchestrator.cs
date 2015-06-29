//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using WinSSC.Macros;
using WinSSC.ArticleProcessors;
using WinSSC.TemplateProcessors;
using WinSSC.VirtualArticleGenerators;

namespace WinSSC
{
    /// <summary>
    /// Runs the whole conversion operation
    /// </summary>
    class Orchestrator
    {
        private const string TEMPLATE_ATTR_KEY = "Template";
        private const string PATH_ATTR_KEY = "Path";

        AttributeSet validAttributes;
        IList<IMacroProvider> macroProviders;
        Config.Paths paths;

        /// <summary>
        /// Creates Orchestrator using default options for configuration (loaded from file system).
        /// </summary>
        public Orchestrator()
            : this(Config.LoadPaths(), AttributeSet.LoadFromConfig())
        {
        }

        public Orchestrator(
            Config.Paths paths, 
            AttributeSet attributes) :this(paths, attributes, getDefaultMacroProviders(paths, attributes))
        {
        }

        /// <summary>
        /// Dependency injected constructor, mainly to support unit testing.
        /// </summary>
        /// <param name="paths">Paths to key items. Will not be used as heavily as in null constructor, as most config is provided.</param>
        /// <param name="attributes">A list of valid attributes to use in place of loading directly from config.</param>
        /// <param name="macroProviders">A list of macro providers to use instead of default.</param>
        public Orchestrator(
            Config.Paths paths, 
            AttributeSet attributes, 
            IList<IMacroProvider> macroProviders) 
        {
            this.paths = paths;
            this.validAttributes = attributes;
            this.macroProviders = macroProviders;
        }

        /// <summary>
        /// Sets up all known macro providers for use.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static IList<IMacroProvider> getDefaultMacroProviders(Config.Paths paths, AttributeSet validAttributes)
        {
            List<IMacroProvider> macroProviders = new List<IMacroProvider>();
            if (paths == null) return macroProviders;

            macroProviders.Add(new MarkdownMacroProvider(paths.MacrosRootDir));
            macroProviders.Add(new BasicTextMacroProvider(paths.MacrosRootDir));
            macroProviders.Add(new XslMacroProvider(paths.MacrosRootDir, validAttributes));
            return macroProviders;
        }


        /// <summary>
        /// Top level function of the site compilation/conversion process
        /// </summary>
        public bool ConvertArticles()
        {
            List<ArticleDto> articles = new List<ArticleDto>();
            IList<ITemplateProcessor> templateProcessors;
            List<TemplateDto> templates = new List<TemplateDto>();
            List<IVirtualArticleGenerator> articleGenerators = new List<IVirtualArticleGenerator>();
            
            string[] articlePaths;

            templateProcessors = new List<ITemplateProcessor>();
            templateProcessors.Add(new MarkdownTemplateProcessor(validAttributes));


            if (validAttributes == null) return false;

            //Load articles
            //At this point in time, we only have one type of article reader...
            IArticleProcessor articleProcessor = new MarkdownArticleProcessor(validAttributes);
            articlePaths = Directory.GetFiles(paths.ArticlesRootDir, "*" + articleProcessor.PrimaryFileExtension);
            foreach(string path in articlePaths)
            {
                string relativePath = path.Substring(paths.ArticlesRootDir.Length);
                ArticleDto art = articleProcessor.ParseArticleRawText(File.ReadAllText(path), relativePath);
                if (art != null)
                {
                    articles.Add(art);
                }
            }

            //Load templates
            foreach(ITemplateProcessor tp in templateProcessors)
            {
                string[] templatePaths = Directory.GetFiles(paths.TemplatesRootDir, "*" + tp.PrimaryFileExtension);
                foreach (string path in templatePaths)
                {
                    string relativePath = path.Substring(paths.ArticlesRootDir.Length);
                    TemplateDto template = tp.ParseTemplateRawText(File.ReadAllText(path), relativePath, Path.GetFileNameWithoutExtension(relativePath));
                    if (template != null)
                    {
                        template.TemplateProcessor = tp;
                        templates.Add(template);
                    }
                }
            }

            //Load virtual article generators
            if (!string.IsNullOrEmpty(paths.VirtualArticlesRootDir))
            {
                IVirtualArticleGeneratorLoader loader = new XslMarkdownArticleGeneratorLoader(validAttributes);
                string[] generatorPaths = Directory.GetFiles(paths.VirtualArticlesRootDir, "*" + loader.PrimaryFileExtension);
                foreach (string path in generatorPaths)
                {
                    IVirtualArticleGenerator gen = loader.ParseGeneratorFromFile(path);
                    if (gen != null)
                    {
                        articleGenerators.Add(gen);
                    }
                }
            }

            createVirtualArticles(articles, articleGenerators);

            applyArticleTemplates(articles, templates); 

            applyOutputPaths(articles);

            applyMacros(articles);

            transformArticles(articles);

            outputArticles(articles);

            return true;
        }


        public bool ConvertImages()
        {
            ImageProcessor ip = new ImageProcessor(Config.ConfigDir);
            ip.ConvertImages();
            return true;
        }

        private void outputArticles(List<ArticleDto> articles)
        {
            foreach(ArticleDto article in articles)
            {
                File.WriteAllText(paths.OutputRootDir + article.OutputPath, article.Content);
            }
        }

        private void applyArticleTemplates(List<ArticleDto> articles, List<TemplateDto> templates)
        {
            foreach (ArticleDto article in articles)
            {
                if (article.Attributes.ContainsKey(TEMPLATE_ATTR_KEY))
                {
                    if (article.Attributes[TEMPLATE_ATTR_KEY].Count > 0)
                    {
                        string articleTemplateName = article.Attributes[TEMPLATE_ATTR_KEY][0];
                        TemplateDto template = templates.FirstOrDefault(x => x.Name.ToLower() == articleTemplateName.ToLower());
                        if (template != null)
                        {
                            article.Template = template;
                            article.TemplateContent = template.Content;
                            foreach (var templateAttr in template.Attributes)
                            {
                                if (!article.Attributes.ContainsKey(templateAttr.Key))
                                {
                                    article.Attributes.Add(templateAttr.Key, templateAttr.Value);
                                }
                            }
                        }
                        else
                        {
                            Logger.LogWarning("Article " + article.SourceFileRelativePath + " refers to non-existent template " + articleTemplateName + ". ");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses the virtual article generators to create virtual articles based on those articles already loaded.
        /// </summary>
        /// <param name="articles">Existing articles loaded from disk.</param>
        /// <param name="articleGenerators">Article generators.</param>
        private void createVirtualArticles(List<ArticleDto> articles, List<IVirtualArticleGenerator> articleGenerators)
        {
            List<ArticleDto> newArticles = new List<ArticleDto>();
            foreach(IVirtualArticleGenerator gen in articleGenerators)
            {
                newArticles.AddRange(gen.GenerateArticles(articles));
            }
            articles.AddRange(newArticles);
        }

        /// <summary>
        /// Determines the output path for each article, either using a specified pattern or 
        /// translation of the source file relative path.
        /// </summary>
        /// <param name="articles"></param>
        private void applyOutputPaths(List<ArticleDto> articles)
        {
            //Todo: More sophisticated, flexible path logic outstanding
            foreach(ArticleDto a in articles)
            {
                a.OutputPath = AttributeTranformations.CalculateArticlePath(a);

                //Replace existing path with the calculated path. We can assume Path is always available.
                a.Attributes["Path"] = new List<string>();
                a.Attributes["Path"].Add(a.OutputPath);
            }
        }

        /// <summary>
        /// Expands macros for all articles on the given list.
        /// </summary>
        /// <param name="articles"></param>
        private void applyMacros(IList<ArticleDto> articles)
        {
            foreach(ArticleDto a in articles)
            {
                ProcessArticleMacros(a, articles);

            }
        }

        /// <summary>
        /// Expands all macros found in an article's content. Will also expand macros added by macros.
        /// </summary>
        /// <param name="a">The article in question. Content will be updated.</param>
        /// <param name="articles">List of all known articles.</param>
        public void ProcessArticleMacros(ArticleDto a, IList<ArticleDto> articles)
        {
            IList<MacroInvocation> invokes = null;

            //This loop is just to allow an escape from an infinite macro expansion cycle
            for(int cycle = 0; cycle < 20; cycle++)
            {
                invokes = a.ArticleProcessorUsed.LocateMacrosInContent(a);
                invokes = new List<MacroInvocation>(invokes.OrderByDescending(x => x.StartingCharIndex));
                foreach(MacroInvocation mi in invokes)
                {
                    IMacro mac = null;
                    foreach(var provider in macroProviders)
                    {
                        mac = provider.TryLoadMacro(mi.MacroName);
                        if (mac != null) break;
                    }
                    if(mac == null)
                    {
                        Logger.LogWarning("Macro " + mi.MacroName + " not found.");
                        continue;
                    }

                    //TODO: This is not the most efficient way to handle progressive replace. Should use stringbuilders.
                    string pre = a.Content.Substring(0, mi.StartingCharIndex);
                    string post = a.Content.Substring(mi.EndingCharIndex);
                    a.Content = pre + mac.Expand(mi.Parameters, mi.Values, a, articles) + post;
                }
                if (invokes.Count == 0) break;
            }

            if (invokes.Count > 0) Logger.LogWarning("Article " + a.SourceFileRelativePath + " still had macros to expand after many passes. Check for macros recurively including each other.");

            if(a.Template != null)
            {
                for(int cycle = 0; cycle < 20; cycle++)
                {
                    invokes = a.Template.TemplateProcessor.LocateMacrosInArticleTemplateContent(a);
                    invokes =  new List<MacroInvocation> (invokes.OrderByDescending(x => x.StartingCharIndex));
                    foreach (MacroInvocation mi in invokes)
                    {
                        IMacro mac = null;
                        foreach (var provider in macroProviders)
                        {
                            mac = provider.TryLoadMacro(mi.MacroName);
                            if (mac != null) break;
                        }
                        if (mac == null)
                        {
                            Logger.LogWarning("Macro " + mi.MacroName + " not found.");
                            continue;
                        }

                        //TODO: This is not the most efficient way to handle progressive replace. Should use stringbuilders.
                        string pre = a.TemplateContent.Substring(0, mi.StartingCharIndex);
                        string post = a.TemplateContent.Substring(mi.EndingCharIndex);
                        a.TemplateContent = pre + mac.Expand(mi.Parameters, mi.Values, a, articles) + post;
                    }
                }
                if (invokes.Count > 0) Logger.LogWarning("Article template for " + a.SourceFileRelativePath + " still had macros to expand after many passes. Check for macros recurively including each other.");
            }
        }

        private void transformArticles(IList<ArticleDto> articles)
        {
            foreach(ArticleDto art in articles)
            {
                art.ArticleProcessorUsed.FinaliseArticleHtml(art);
                if(art.Template != null)
                {
                    art.Template.TemplateProcessor.WrapArticle(art);
                }
            }
        }
    }
}
