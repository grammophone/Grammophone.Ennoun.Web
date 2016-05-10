﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grammophone.Ennoun.Web.Controllers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class InferenceMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal InferenceMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Grammophone.Ennoun.Web.Controllers.InferenceMessages", typeof(InferenceMessages).Assembly);
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
        ///   Looks up a localized string similar to There has been an error while starting the inference engine. Infernce will not be possible..
        /// </summary>
        internal static string ENGINE_LOADING_ERROR {
            get {
                return ResourceManager.GetString("ENGINE_LOADING_ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The inference engine has not yet started. Please wait a few minutes until the upper right engine status turns into a green OK sign..
        /// </summary>
        internal static string ENGINE_NOT_YET_STARTED {
            get {
                return ResourceManager.GetString("ENGINE_NOT_YET_STARTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This doesn&apos;t seem a greek sentence. No inference is performed..
        /// </summary>
        internal static string SENTENCE_IMPOSSIBLE {
            get {
                return ResourceManager.GetString("SENTENCE_IMPOSSIBLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The inference probability of the sentence is low. Errors are likely..
        /// </summary>
        internal static string SENTENCE_PROBABILITY_LOW {
            get {
                return ResourceManager.GetString("SENTENCE_PROBABILITY_LOW", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sentence does not contain a terminating punctuation mark..
        /// </summary>
        internal static string SENTENCE_PUNCTUATION_MISSING {
            get {
                return ResourceManager.GetString("SENTENCE_PUNCTUATION_MISSING", resourceCulture);
            }
        }
    }
}
