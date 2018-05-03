import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'search-results',
    templateUrl: './searchresults.component.html'
})
export class SearchResultsComponent {
    public searchResults: SearchResult[];
    public isLoading: Boolean = false;
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.getMoreResults();
    }

    getMoreResults() {
        if (this.isLoading) return;
        this.isLoading = true;

        this.http.get(this.baseUrl + 'api/Search/Search', {
            params: {
                skip: this.searchResults ? this.searchResults.length : 0,
                take: 30
            }
        }).subscribe(result => {
            var resultsPage = result.json() as SearchResult[];
            if (!this.searchResults) this.searchResults = resultsPage;
            else {
                for (var r in resultsPage) this.searchResults.push(resultsPage[r]);
            }
            this.isLoading = false;
        }, error => {
            console.error(error);
            this.isLoading = false;
        });
    }
}

interface SearchResult {
    incipitSvg: string;
}