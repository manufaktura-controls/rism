var SearchViewModel = function () {
    var self = this;
    this.pageSize = 30;
    this.results = ko.observableArray([]);
    this.player = new PlaybackManager();
    this.isLoading = ko.observable(false);
    this.mayHaveMoreResults = true;
    this.currentQuery = { intervals: [], skip: 0, take: self.pageSize };

    this.startNewSearch = function (query) {
        if (self.isLoading()) return;

        self.mayHaveMoreResults = true;
        self.currentQuery = query;
        self.results([]);
        self.getMoreResults();
    }

    this.getMoreResults = function () {
        if (self.isLoading() || !self.mayHaveMoreResults) return;

        self.isLoading(true);
        $.ajax({
            type: "POST",
            url: 'api/Search/Search',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(self.currentQuery)
        }).done(function (response) {
            self.isLoading(false);
            self.mayHaveMoreResults = response.results.length == self.pageSize;
            
            for (var i in response.results) {
                var result = response.results[i];
                self.results.push(result);
            }
            self.currentQuery.skip = self.results().length;
            console.info('Query finished in ' + response.queryTime + ' ms.');
        });
    }

    this.getRismLink = function (id) {
        return 'https://opac.rism.info/search?id=' + id + '&View=rism';
    }

    this.formatRelevance = function (relevance) {
        return (relevance * 100) + " %";
    }
}