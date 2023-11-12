window.browserResize = {
    resized: function () {
        DotNet.invokeMethodAsync("Presentation", 'OnBrowserResize', window.innerWidth)
            .then(data => { console.log("Browser resized.") });
    },
    subscribeToResizeEvent: function () {
        window.addEventListener("resize", browserResize.resized);
        console.log("Subnscribed to resize event.");
    },
}