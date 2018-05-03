import { Directive, ElementRef, Input, HostListener, EventEmitter, Output } from "@angular/core";

export type InfiniteScrollContext = 'self' | 'document';
export interface Viewport {
    h: number;
    w: number;
}

@Directive({ selector: '[infiniteScroll]' })
// Directive class
export class InfiniteScrollDirective {
    el: any;
    viewport: Viewport;
    canTriggerAction: boolean = true;

    constructor(private element: ElementRef) {
        this.el = element.nativeElement;
    }

    @Input() infiniteScrollContext: InfiniteScrollContext = 'document';
    @Input() infiniteScrollThreshold: number = 50;
    @Output() infiniteScrollAction: EventEmitter<any> = new EventEmitter();
    @HostListener('scroll', ['$event']) onElementScroll() {
        if (this.infiniteScrollContext === 'self') {
            if (this.elementEndReachedInSelfScrollbarContext() && this.canTriggerAction) {
                this.triggerAction();
            }
        }
    }

    ngOnInit() {
        this.viewport = this.getViewport(window);
        if (this.infiniteScrollContext === 'document') {
            document.addEventListener('scroll', () => {
                if (this.elementEndReachedInDocumentScrollbarContext(window, this.el) && this.canTriggerAction) {
                    console.info('Trigger action');
                    this.triggerAction();
                }
            });
        }
    }

    triggerAction() {
        this.canTriggerAction = false;
        this.infiniteScrollAction.emit(null);
    }

    elementEndReachedInSelfScrollbarContext(): boolean {
        if (this.el.scrollTop + this.el.offsetHeight >= this.el.scrollHeight) {
            this.canTriggerAction = true;
            return true;
        }

        return false;
    }

    elementEndReachedInDocumentScrollbarContext(win: Window, el: any): boolean {
        const rect = el.getBoundingClientRect();
        const scrollableDistance = el.offsetHeight + rect.top + win.pageYOffset;
        const currentPos = win.pageYOffset + this.viewport.h;

        if (currentPos > scrollableDistance) {
            this.canTriggerAction = true;
            return true;
        }

        return false;
    }

    private getViewport(win: Window): Viewport {
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