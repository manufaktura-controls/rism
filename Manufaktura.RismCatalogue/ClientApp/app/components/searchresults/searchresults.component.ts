import { Component, Inject, NgZone } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'search-results',
    templateUrl: './searchresults.component.html'
})
export class SearchResultsComponent {
    public searchResults: SearchResult[];
    public isLoading: Boolean = false;
    private hasMoreResults: boolean = true;
    private http: Http;
    private baseUrl: string;
    private pageSize: number = 30;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string, private ngZone: NgZone) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.getMoreResults();
    }

    getMoreResults() {
        if (this.isLoading || !this.hasMoreResults) return;
        this.ngZone.run(() => { this.isLoading = true; });

        this.http.get(this.baseUrl + 'api/Search/Search', {
            params: {
                skip: this.searchResults ? this.searchResults.length : 0,
                take: this.pageSize
            }
        }).subscribe(result => {
            this.ngZone.run(() => {
                var resultsPage = result.json() as SearchResult[];
                if (!this.searchResults) this.searchResults = resultsPage;
                else {
                    for (var r in resultsPage) this.searchResults.push(resultsPage[r]);
                }
                this.isLoading = false;
                this.hasMoreResults = resultsPage.length == this.pageSize;
            });
        }, error => {
            console.error(error);
                this.ngZone.run(() => { this.isLoading = false; });
        });
    }
}

interface SearchResult {
    incipitSvg: string;
}