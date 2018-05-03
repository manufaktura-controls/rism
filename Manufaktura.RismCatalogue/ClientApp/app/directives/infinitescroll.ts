import { Directive, Renderer, ElementRef } from "@angular/core";

@Directive({ selector: '[infiniteScroll]' })
// Directive class
export class InfiniteScrollDirective {
    constructor(el: ElementRef, renderer: Renderer) {
    }
}