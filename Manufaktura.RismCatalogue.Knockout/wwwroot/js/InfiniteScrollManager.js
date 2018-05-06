var InfiniteScrollManager = function (element, threshold, action) {
    var self = this;
    this.element = element;
    this.infiniteScrollThreshold = threshold;
    this.viewport = getViewport(window);
    this.action = action;
    this.canTriggerAction = true;

    window.addEventListener('scroll', function () { self.scrolled(); });

    this.scrolled = function () {
        if (elementEndReachedInDocumentScrollbarContext() && self.canTriggerAction) {
            self.triggerAction();
        }
    };

    this.triggerAction = function () {
        self.canTriggerAction = false;
        self.action();
    }

    function elementEndReachedInSelfScrollbarContext() {
        if (self.element.scrollTop + self.element.offsetHeight >= self.element.scrollHeight) {
            self.canTriggerAction = true;
            return true;
        }

        return false;
    }

    function elementEndReachedInDocumentScrollbarContext() {
        var rect = self.element.getBoundingClientRect();
        var scrollableDistance = self.element.offsetHeight + rect.top + window.pageYOffset;
        var currentPos = window.pageYOffset + self.viewport.h;

        if (currentPos > scrollableDistance - self.infiniteScrollThreshold) {
            self.canTriggerAction = true;
            return true;
        }

        return false;
    }

    function getViewport(win) {
        // This works for all browsers except IE8 and before
        if (win.innerWidth != null) {
            return {
                w: win.innerWidth,
                h: win.innerHeight
            };
        }

        // For IE (or any browser) in Standards mode
        let d = win.document;

        if (document.compatMode == 'CSS1Compat') {
            return {
                w: d.documentElement.clientWidth,
                h: d.documentElement.clientHeight
            };
        }

        // For browsers in Quirks mode
        return {
            w: d.body.clientWidth,
            h: d.body.clientHeight
        };


    }
}