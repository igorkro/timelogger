﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeLog.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TimeLog.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon MainIcon {
            get {
                object obj = ResourceManager.GetObject("MainIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon MainIconError {
            get {
                object obj = ResourceManager.GetObject("MainIconError", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html []&gt;
        ///&lt;html&gt;
        ///  &lt;head&gt;
        ///    &lt;meta charset=&quot;UTF-8&quot; /&gt;
        ///    &lt;meta name=&quot;author&quot; content=&quot;MarkdownViewer++&quot; /&gt;
        ///    &lt;title&gt;readme.md&lt;/title&gt;
        ///    &lt;style type=&quot;text/css&quot;&gt;
        ///            
        ////* Avoid page breaks inside the most common attributes, especially for exports (i.e. PDF) */
        ///td, h1, h2, h3, h4, h5, p, ul, ol, li {
        ///    page-break-inside: avoid; 
        ///}
        ///
        ///body {
        ///  font-family: &quot;Roboto&quot;,Verdana,sans-serif;
        ///}
        ///
        ///        &lt;/style&gt;
        ///  &lt;/head&gt;
        ///  &lt;body&gt;
        ///    &lt;h1 id=&quot;time-logger&quot;&gt;Time Logger&lt;/h1&gt;
        ///     [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string readme {
            get {
                return ResourceManager.GetString("readme", resourceCulture);
            }
        }
    }
}