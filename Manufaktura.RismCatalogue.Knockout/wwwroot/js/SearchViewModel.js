var SearchViewModel = function () {
    var self = this;
    this.pageSize = 30;
    this.results = ko.observableArray([]);
    this.player = new PlaybackManager();
    this.isLoading = ko.observable(false);
    this.hasMoreResults = true;
    this.currentQuery = { intervals: [], skip: 0, take: self.pageSize };

    this.startNewSearch = function (query) {
        if (self.isLoading()) return;

        self.currentQuery = query;
        self.results([]);
        self.getMoreResults();
    }

    this.getMoreResults = function () {
        if (self.isLoading()) return;

        self.isLoading(true);
        $.ajax({
            type: "POST",
            url: 'api/Search/Search',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(self.currentQuery)
        }).done(function (response) {
            self.isLoading(false);
            self.hasMoreResults = response.length == self.pageSize;
            for (var i in response) {
                var result = response[i];
                self.results.push(result);
            }
        });
    }
}