<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<body>
				<h3 style='color:#ec1010;'>
        	<xsl:for-each select = 'ArrayOfServer/Server'>    
            <xsl:value-of select = "Name"/>
          		 <hr align='left' width='400px'/>
            <table>
              <tr bgcolor = '#04AA6D'>
                <th style = 'background-color:#04AA6D; color:white; align-items:left; padding:5px;'> File Name </th>
                <th style = 'background-color:#04AA6D; color:white; align-items:left; padding:5px;'> Last Update Date</th>
              </tr>
            
             	<xsl:for-each select = 'Files/File'>    
                  <tr>
             		<td style='align-items:left; padding:5px;'>
                  <xsl:value-of select = "FileName"/>
								</td>
                
								<td style='align-items:left; padding:5px;'>
									<xsl:value-of select = "LastUpdateDate"/>
								</td> 
              </tr>
			      	</xsl:for-each>
            
            </table>
            <br></br>
              
          </xsl:for-each>
         </h3>
        
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>