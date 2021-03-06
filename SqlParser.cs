using System;
using System.Text;
using System.Xml;

namespace SQLParse
{
    public class SqlParser : ParserBase
    {
    
        /// <summary>
        /// Stores the types of all the possible tags.
        /// </summary>
        Type[] fTags;

        /// <summary>
        /// Indicates whether the white space is a non-valueable character.
        /// </summary>
        protected override bool IsSkipWhiteSpace
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the list of all the available tags.
        /// </summary>
        protected override Type[] Tags
        {
            get
            {
                if (fTags == null)
                {
                    fTags = new Type[] {
                        typeof(SelectTag),
                        typeof(FromTag),
                        typeof(WhereTag),
                        typeof(OrderByTag),
                        typeof(BracesTag),
                        typeof(StringLiteralTag),
                        typeof(ForUpdateTag),
                        typeof(StartWith),
                        typeof(GroupByTag),
                        typeof(QuotedIdentifierTag)
                    };
                }

                return fTags;
            }
        }

    

    

        /// <summary>
        /// Returns the xml node which corresponds to the From tag.
        /// If this node does not exist, this method generates an
        /// exception.
        /// </summary>
        private XmlNode GetFromTagXmlNode()
        {
            XmlNode myFromNode = ParsedDocument.SelectSingleNode(string.Format(@"{0}/{1}[@{2}='{3}']", cRootXmlNodeName, cTagXmlNodeName, cTagTypeXmlAttributeName, FromTag.cTagName));
            if (myFromNode == null)
                throw new Exception(ToText());

            return myFromNode;
        }

        /// <summary>
        /// Returns the xml node which corresponds to the For Update tag.	
        /// </summary>
        private XmlNode GetForUpdateTagXmlNode()
        {
            XmlNode myForUpdateNode = ParsedDocument.SelectSingleNode(string.Format(@"{0}/{1}[@{2}='{3}']", cRootXmlNodeName, cTagXmlNodeName, cTagTypeXmlAttributeName, ForUpdateTag.cTagName));

            return myForUpdateNode;
        }

        /// <summary>
        /// Checks whether there is a tag in the text at the specified position, and returns its tag.
        /// </summary>
        internal new Type IsTag(string text, int position)
        {
            return base.IsTag(text, position);
        }

    

        /// <summary>
        /// Returns the xml node which corresponds to the Where tag.
        /// If this node does not exist, creates a new one (if needed).
        /// </summary>
        private XmlNode GetWhereTagXmlNode(bool createNew)
        {
            XmlNode myWhereNode = ParsedDocument.SelectSingleNode(string.Format(@"{0}/{1}[@{2}='{3}']", cRootXmlNodeName, cTagXmlNodeName, cTagTypeXmlAttributeName, WhereTag.cTagName));
            if (myWhereNode == null && createNew)
            {
                WhereTag myWhereTag = new WhereTag();
                myWhereTag.InitializeFromData(this, null, false);
                myWhereNode = CreateTagXmlNode(myWhereTag);

                XmlNode myFromNode = GetFromTagXmlNode();
                myFromNode.ParentNode.InsertAfter(myWhereNode, myFromNode);
            }

            return myWhereNode;
        }

    
        /// <summary>
        /// Gets or sets the Where clause of the parsed sql.
        /// </summary>
        public string WhereClause
        {
            get
            {
            
                XmlNode myWhereTagXmlNode = GetWhereTagXmlNode(false);
                if (myWhereTagXmlNode == null)
                    return string.Empty;

            

                StringBuilder myStringBuilder = new StringBuilder();
                XmlNodesToText(myStringBuilder, myWhereTagXmlNode.ChildNodes);
                return myStringBuilder.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                
                    XmlNode myWhereXmlNode = GetWhereTagXmlNode(false);
                    if (myWhereXmlNode != null)
                        myWhereXmlNode.ParentNode.RemoveChild(myWhereXmlNode);
                }
                else
                {
                

                    XmlNode myWhereXmlNode = GetWhereTagXmlNode(true);

                    ClearXmlNode(myWhereXmlNode);

                    TagBase myWhereTag = TagXmlNodeToTag(myWhereXmlNode);

                    ParseBlock(myWhereXmlNode, myWhereTag, value, 0);
                }
            }
        }

        

        /// <summary>
        /// Returns the xml node which corresponds to the Order By tag.
        /// If this node does not exist, creates a new one (if needed).
        /// </summary>
        private XmlNode GetOrderByTagXmlNode(bool createNew)
        {
            XmlNode myOrderByNode = ParsedDocument.SelectSingleNode(string.Format(@"{0}/{1}[@{2}='{3}']", cRootXmlNodeName, cTagXmlNodeName, cTagTypeXmlAttributeName, OrderByTag.cTagName));
            if (myOrderByNode == null && createNew)
            {
                OrderByTag myOrderByTag = new OrderByTag();
                myOrderByTag.InitializeFromData(this, null, false);
                myOrderByNode = CreateTagXmlNode(myOrderByTag);

                XmlNode myForUpdateNode = GetForUpdateTagXmlNode();
                if (myForUpdateNode != null)
                {
                    myForUpdateNode.ParentNode.InsertBefore(myOrderByNode, myForUpdateNode);
                    return myOrderByNode;
                }

                XmlNode myFromNode = GetFromTagXmlNode();
                myFromNode.ParentNode.AppendChild(myOrderByNode);
            }

            return myOrderByNode;
        }

    

        /// <summary>
        /// Gets or sets the Order By clause of the parsed sql.
        /// </summary>
        public string OrderByClause
        {
            get
            {
    

                XmlNode myOrderByTagXmlNode = GetOrderByTagXmlNode(false);
                if (myOrderByTagXmlNode == null)
                    return string.Empty;

    

                StringBuilder myStringBuilder = new StringBuilder();
                XmlNodesToText(myStringBuilder, myOrderByTagXmlNode.ChildNodes);
                return myStringBuilder.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                

                    XmlNode myOrderByXmlNode = GetOrderByTagXmlNode(false);
                    if (myOrderByXmlNode != null)
                        myOrderByXmlNode.ParentNode.RemoveChild(myOrderByXmlNode);


                }
                else
                {
                

                    XmlNode myOrderByXmlNode = GetOrderByTagXmlNode(true);

                    ClearXmlNode(myOrderByXmlNode);

                    TagBase myOrderByTag = TagXmlNodeToTag(myOrderByXmlNode);

                    ParseBlock(myOrderByXmlNode, myOrderByTag, value, 0);

                }
            }
        }

    }
}
