using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public class CodeTree
    {
        public Dictionary<string, List<string>> CodesWithPossibleMatches = new Dictionary<string, List<string>>();
        public CodeTree(List<string> distinctCodes)
            : this(distinctCodes, 0)
        {
            foreach (string distinctCode in distinctCodes)
            {
                List<string> possibleMatches = new List<string>();
                string match = distinctCode;
                while (match.Length > 0)
                {
                    possibleMatches.Add(match);
                    match = match.Substring(0, match.Length - 1);
                }
                CodesWithPossibleMatches.Add(distinctCode, possibleMatches);
            }
        }

        public CodeTree(List<string> distinctCodes, int comparePosition)
        {
            this._comparePosition = comparePosition;
            this.MatchCodes = distinctCodes;
            BuildChildNodes();
        }

        public Dictionary<char,CodeNode> ChildNodes { get; private set; }
        public List<string> MatchCodes { get; private set; }
        private int _comparePosition;

        void BuildChildNodes()
        {
            this.ChildNodes = new Dictionary<char, CodeNode>();
            char currentNodeDigit = char.MinValue;
            List<string> childNodeCodes = new List<string>();
            foreach (var code in this.MatchCodes.OrderBy(itm => itm))
            {
                if (code.Length > this._comparePosition &&
                    (code[this._comparePosition] != currentNodeDigit || currentNodeDigit == char.MinValue))
                {
                    if (currentNodeDigit != char.MinValue)
                        this.ChildNodes.Add(currentNodeDigit, new CodeNode(currentNodeDigit, childNodeCodes, this._comparePosition + 1));
                    currentNodeDigit = code[this._comparePosition];
                    childNodeCodes = new List<string>();
                }

                if (code.Length > 1)
                    childNodeCodes.Add(code);
            }
            //add last node
            if(currentNodeDigit != char.MinValue)
                this.ChildNodes.Add(currentNodeDigit, new CodeNode(currentNodeDigit, childNodeCodes, this._comparePosition + 1));
        }

        public List<string> GetCodesThatMatch(string code)
        {
            CodeNode matchChild;
            if (this.ChildNodes.TryGetValue(code[this._comparePosition], out matchChild))
            {
                if (code.Length - 1 == this._comparePosition)
                    return matchChild.MatchCodes;
                else
                    return matchChild.GetCodesThatMatch(code);
            }
            else
                return null;
        }

        public string GetLongestMatch(string code)
        {
            if (code.Length == this._comparePosition)
                if (this.MatchCodes.Contains(code))
                    return code;
                else 
                    return null;
            else
            {
                CodeNode matchChild;
                if (this.ChildNodes.TryGetValue(code[this._comparePosition], out matchChild))
                    return matchChild.GetLongestMatch(code);
                else
                {
                    string supposedMatch = code.Substring(0, this._comparePosition);
                    if (this.MatchCodes.Contains(supposedMatch))
                        return supposedMatch;
                    else
                        return null;
                }
                    
            }
        }
    }
}
