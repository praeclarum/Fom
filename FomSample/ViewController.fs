namespace FomSample

open System

open Foundation
open UIKit

open TodoDomain

[<Register("ViewController")>]
type ViewController(handle : IntPtr) =
    inherit UIViewController(handle)

    let emptyItem : TodoItem = { Title = ""; Completed = None; Important = false }
    let aItem = { emptyItem with Title = "A" }
    let bItem = { emptyItem with Title = "B" }

    override x.DidReceiveMemoryWarning() =
        // Releases the view if it doesn't have a superview.
        base.DidReceiveMemoryWarning()
    // Release any cached data, images, etc that aren't in use.

    override x.ViewDidLoad() =
        base.ViewDidLoad()
        let diff = aItem - bItem
        printfn "%A" diff

    override x.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation) =
        // Return true for supported orientations
        if UIDevice.CurrentDevice.UserInterfaceIdiom = UIUserInterfaceIdiom.Phone then
            toInterfaceOrientation <> UIInterfaceOrientation.PortraitUpsideDown
        else true
