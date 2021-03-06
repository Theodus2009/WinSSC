#WinSSC - Static Site Compiler for Windows

This project aims to build an easy to use, ready to run out-of-box static site compiler for Windows. It is intended to be run from the command line as part of a site build/deploy script, so this tool focusses purely on creating output HTML and optionally resizing images to fit a pre-determined list of sizes. Plain old file copy functions are deliberately omitted.

This tool is written in C# targeting .Net 4.0, using Visual Studio 2013.
Site content is written in Markdown, with dynamic content implemented in the form of macros. Further details on usage are at a post introducing the tool at [my site](http://eddiesoft.id.au/Static%20Site%20Builder%20v0.8.html).
Should the linked post be unavailable, see WinSSC Intro - from website.pdf

License: GNU [Lesser-GPL v3](https://www.gnu.org/licenses/lgpl-3.0.en.html)