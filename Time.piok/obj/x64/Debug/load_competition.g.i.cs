﻿#pragma checksum "..\..\..\load_competition.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "073086476C86F2B36C3ED7CF551DD8C2"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34014
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Time.piok {
    
    
    /// <summary>
    /// load_competition
    /// </summary>
    public partial class load_competition : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\load_competition.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listview;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\load_competition.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btn_add_competition;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\load_competition.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btn_load_competition;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\load_competition.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btn_remove_cat;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\load_competition.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btn_close;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Time.piok;component/load_competition.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\load_competition.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.listview = ((System.Windows.Controls.ListView)(target));
            return;
            case 2:
            this.btn_add_competition = ((System.Windows.Controls.MenuItem)(target));
            
            #line 16 "..\..\..\load_competition.xaml"
            this.btn_add_competition.Click += new System.Windows.RoutedEventHandler(this.btn_add_competition_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btn_load_competition = ((System.Windows.Controls.MenuItem)(target));
            
            #line 17 "..\..\..\load_competition.xaml"
            this.btn_load_competition.Click += new System.Windows.RoutedEventHandler(this.btn_load_competition_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btn_remove_cat = ((System.Windows.Controls.MenuItem)(target));
            
            #line 18 "..\..\..\load_competition.xaml"
            this.btn_remove_cat.Click += new System.Windows.RoutedEventHandler(this.btn_remove_cat_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btn_close = ((System.Windows.Controls.MenuItem)(target));
            
            #line 19 "..\..\..\load_competition.xaml"
            this.btn_close.Click += new System.Windows.RoutedEventHandler(this.btn_close_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

