import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'search-results',
    templateUrl: './searchresults.component.html'
})
export class SearchResultsComponent {
    public searchResults: SearchResult[] = [];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/Search/Search', {
            params: {
                skip: 0, 
                take: 20
            }}).subscribe(result => {
            var resultsPage = result.json() as SearchResult[];
            for (var r in resultsPage) this.searchResults.push(resultsPage[r]);
        }, error => console.error(error));
    }
}

interface SearchResult {
    incipitSvg: string;
}
