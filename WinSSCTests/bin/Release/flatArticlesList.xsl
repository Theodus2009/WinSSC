<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="text" omit-xml-declaration="yes" />

  <xsl:template match="/">
    <xsl:for-each select="Articles/Article">
      <xsl:value-of select="Title"/>
      <xsl:text>
</xsl:text>
    </xsl:for-each>
  </xsl:template>

</xsl:transform>