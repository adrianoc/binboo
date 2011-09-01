/**
 * Copyright (c) 2011 Adriano Carlos Verona
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **/
using Binboo.Core.Commands.Arguments;

namespace Binboo.Jira.Commands
{
    class JiraParamValidator : ParamValidator
    {
        protected JiraParamValidator(string regex, bool optional) : base(regex, optional)
        {
        }

        protected JiraParamValidator(string regex, params ParamValidator[] validators) : base(regex, validators)
        {
        }

        internal static readonly ParamValidator ProjectBase = new JiraParamValidator("\\b[A-Za-z]{3,4}");
        public static readonly ParamValidator Project = new JiraParamValidator("%0%(?=$|\\s+)", ProjectBase);
        public static readonly ParamValidator IssueId = new JiraParamValidator(@"%0%-[0-9]+", ProjectBase);
        public static readonly ParamValidator IssueStatus = new JiraParamValidator("open|closed|all");
        public static readonly ParamValidator Iteration = new JiraParamValidator("(?<param>[0-9]+)", true);
        public static readonly ParamValidator MultipleIssueId = new JiraParamValidator(@"(?<issues>(?<param>[A-Za-z]{1,4}-[0-9]{1,4})(\s*,\s*(?<param>[A-Za-z]{1,4}-[0-9]{1,4}))*)");
        public static readonly ParamValidator Order = new JiraParamValidator(@"\b0?[1-9]\b");
        public static readonly ParamValidator Peer = AnythingStartingWithText.AsOptional();
        public static readonly ParamValidator Type = new JiraParamValidator(@"type\s*=\s*(?<param>bug|task|improvement|b|t|i|.*)\z", true);
        public static readonly ParamValidator UserName = new JiraParamValidator(@"\s*(?<param>[A-za-z][A-Za-z_]*[0-9]*)", true);
        public static readonly ParamValidator Estimation = new JiraParamValidator(@"[0-9]+");
        public static readonly ParamValidator Version = new JiraParamValidator(@"\bversions\s*=(\s*(?<param>[1-9]?[0-9]*((\.)?(?(3)[0-9]+|\b))+)(\s*,?\s*)?(?(4)(?=\s*[0-9]|\b)))+");
        public static readonly ParamValidator LabelOperations = new JiraParamValidator(@"(?<labels>(?<param>(?:(?:\B\+|\B\-)[^\s]*\b))|(\s*(?<param>(?:(?:\B\+|\B\-)[^\s]*\b)))+)", true);
    }
}
