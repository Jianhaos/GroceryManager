﻿

#pragma checksum "C:\Users\songj_000\Documents\Visual Studio 2013\Projects\Grocery Master\Grocery Master\StorageListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4B9B0BF5ACC5A6D0157A7E4191388DBE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grocery_Master
{
    partial class StorageListPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 16 "..\..\StorageListPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SelectionButton_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 17 "..\..\StorageListPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.DeleteButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 60 "..\..\StorageListPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

