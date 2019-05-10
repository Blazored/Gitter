using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

//
//  Components that want to be modal should do the following:
//
//  1. Wrap all the contents in one of these
//  2. Set a ref on the wrapper to identify it
//  3. Create a component ref capture so that they can interact with the wrapper
//  4. In their post render, they shoul call our InvokeModal() method and pass
//     in a popup id that identifies that popup (SelectFooBarPopup, that kind of
//     thing.
//  5. Upon dismissal, they should call our DismissModal with the same popup id
//
//  Then of course presumably upon return to the invoker of the popup, it will
//  re-render without the popup contents.
//
namespace Blazor.Gitter.Components.BGModal
{
    public class BGModal : ComponentBase
    {
        [Inject] IJSRuntime jsRuntime { get; set; }

        public BGModal()
        {
        }

        //
        //  The containing component must call this when he's done. If this is the
        //  last layer being removed, we take the body out of modal state.
        //
        public void DismissModal(string popupId)
        {
            if (EntryStack.Count == 0)
                throw new InvalidOperationException("The modal stack is empty on dismiss");

            // Make sure the one being popped is the top
            if (EntryStack.Peek() != popupId)
            {
                throw new InvalidOperationException
                (
                    "Modal popup " + popupId + " is being poppped, but the top of modal stack is " +
                    EntryStack.Peek()
                );
            }

            // Looks ok, so pop it. If this is the last one, take us out of modal state
            EntryStack.Pop();
            if (EntryStack.Count == 0)
                BGDOMHelpers.setModalState(jsRuntime, false);
        }

        //
        //  The containing component must call this (from his OnAfterRender since
        //  the component ref he needs won't be available till then. We add his
        //  popup id to the stack if it's not the top already. If it's the first
        //  entry, we will put the body into modal state.
        //
        public void InvokeModal(string popupId)
        {
            // If this is not a redundant invocation of the same popup, then push it
            if ((EntryStack.Count == 0) || (EntryStack.Peek() != popupId))
            {
                EntryStack.Push(popupId);

                // If this is the first layer, then set up modal state
                if (EntryStack.Count == 1)
                    BGDOMHelpers.setModalState(jsRuntime, true);
            }
        }



        // Generate our render output, including the child content fragment that we contain
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            int rendSeq = 1;

            builder.OpenElement(rendSeq++, "div");
            builder.AddAttribute(rendSeq++, "id", "GCModalBgn");

            // Set or another style based on whether we are on the first level or nested
            if (EntryStack.Count > 1)
                builder.AddAttribute(rendSeq++, "class", "gc-modal-bgn2");
            else
                builder.AddAttribute(rendSeq++, "class", "gc-modal-bgn1");

            // Add our child content
            builder.AddContent(rendSeq, ChildContent);

            builder.CloseElement();
        }


        // ----------------------------------------------
        //  We need to track when there are modals visible. We could have nested modals, so we 
        //  can't just use a boolean. We can't even really use a counter, because this is the
        //  browser, where nothing works right. We can get re-rendered multiple times, and 
        //  there doesn't appear to be anything we do in the normal sort of way to maintain a
        //  re-entry counter.
        //
        //  So we implement a stack. Each invocation of modal has to provide an id. We will check
        //  the stack and see if that id is the current top of stack. If so, we assume it is a
        //  redundant invocation. On dimiss, we pop the top of stack.
        // ----------------------------------------------
        [Parameter] RenderFragment ChildContent { get; set; }
        private static Stack<string> EntryStack = new Stack<string>(8);
    }
}
