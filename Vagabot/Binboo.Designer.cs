﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Binboo {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Binboo {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Binboo() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Binboo.Binboo", typeof(Binboo).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assigns an issue to a specific developer
        ///use: %cmd% &lt;ticket #&gt; &lt;main developer&gt;| myself [&lt;peer&gt;| myself] [&lt;iteration&gt;].
        /// </summary>
        internal static string Assign {
            get {
                return ResourceManager.GetString("Assign", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Closes an issue.
        ///Use: %cmd% &lt;ticket #&gt;  fixed|won&apos;t fix|incomplete|cannot reproduce  [comment].
        /// </summary>
        internal static string Close {
            get {
                return ResourceManager.GetString("Close", resourceCulture);
            }
        }
        
        internal static System.Drawing.Icon Connected {
            get {
                object obj = ResourceManager.GetObject("Connected", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shows the sum of IDS for the Early Progress Report.
        ///Use: %cmd% [*all|open|closed].
        /// </summary>
        internal static string CountIDS {
            get {
                return ResourceManager.GetString("CountIDS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Drop the task for the current iteration by reseting assignee/peer/iteration in the correspondent ticket.
        ///Use: %cmd% [&lt;comment&gt;].
        /// </summary>
        internal static string Drop {
            get {
                return ResourceManager.GetString("Drop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Files a new issue into jira system. 
        ///Use: %cmd% &lt;project&gt; &lt;summary&gt; [description] [&lt;order&gt;] [type=*bug|task|improvement].
        /// </summary>
        internal static string File {
            get {
                return ResourceManager.GetString("File", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Help
        ///Use: %cmd% [&lt;command&gt;].
        /// </summary>
        internal static string Help {
            get {
                return ResourceManager.GetString("Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displays the main details about an issue
        ///Use: %cmd% &lt;ticket #&gt; [comments].
        /// </summary>
        internal static string Issue {
            get {
                return ResourceManager.GetString("Issue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displays a list of all projects registered in Jira.
        ///Use: %cmd%.
        /// </summary>
        internal static string ListProjects {
            get {
                return ResourceManager.GetString("ListProjects", resourceCulture);
            }
        }
        
        internal static System.Drawing.Icon NotConnected {
            get {
                object obj = ResourceManager.GetObject("NotConnected", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose pairs randomically.
        /// </summary>
        internal static string Pairs {
            get {
                return ResourceManager.GetString("Pairs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Searchs for issues containing the text.
        ///Use:%cmd% text [all|*open|closed].
        /// </summary>
        internal static string Search {
            get {
                return ResourceManager.GetString("Search", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the order for one or more issues
        ///Use: %cmd% &lt;ticket#&gt; order.
        /// </summary>
        internal static string SetOrder {
            get {
                return ResourceManager.GetString("SetOrder", resourceCulture);
            }
        }
    }
}
