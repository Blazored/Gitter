using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;


//
//  Implements two different components. One is a navigation bar, and the other
//  is for the navigation links inside of it.
//
namespace Blazor.Gitter.Components.BGNavMenu
{ 
    //  We need a derivative of the UI event change parameters class. We pass the
    //  assigned menu item name.
    //
    public class BGNavMenuSelection : UIEventArgs
    {
        public string SelectedItemId { get; set; }
    };


    public class BGNavMenuItemInfo
    {
        public BGNavMenuItemInfo(string id, string text, bool isVisible)
        {
            pItemId = id;
            pItemText = text;
            pItemIsVisible = isVisible;
        }

        public string pItemId { get; set; }
        public string pItemText { get; set; }
        public bool pItemIsVisible { get; set; }
    };

    
    public class OptiNavMenuItem : ComponentBase
    {
        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            int rendSeq = 1;

            builder.OpenElement(rendSeq++, "div");

            if (IsActive)
                builder.AddAttribute(rendSeq++, "class", "gc-navlink-active");
            else
                builder.AddAttribute(rendSeq++, "class", "gc-navlink");

            builder.AddAttribute
            (
                rendSeq++, "onclick", BindMethods.GetEventHandlerValue<UIMouseEventArgs>(e => OnClick(e))
            );
            builder.AddContent(rendSeq++, ItemText);
            builder.CloseElement();
        }


        // We just pass along our configured item name, so we don't need any parameters
        private bool OnClick(UIMouseEventArgs e)
        {
            OnSelect?.Invoke(new BGNavMenuSelection{ SelectedItemId = ItemId });
            return false;
        }


        // We get all of this stuff as parameters from our parent menu class when he generates us.
        [Parameter] private bool   IsActive { get; set; } = false;
        [Parameter] private string ItemId { get; set; }
        [Parameter] private string ItemText{ get; set; }


        // An action we invoke when our selection changes
        [Parameter] private Action<BGNavMenuSelection> OnSelect{ get; set; }
    }
}
