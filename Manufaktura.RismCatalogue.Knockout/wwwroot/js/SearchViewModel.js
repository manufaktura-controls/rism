var SearchViewModel = function () {
    var self = this;
    this.pageSize = 30;
    this.results = ko.observableArray([]);
    this.player = new PlaybackManager();
    this.isLoading = ko.observable(false);
    this.hasMoreResults = true;

    this.getMoreResults = function () {
        self.isLoading(true);
        $.ajax({
            type: "GET",
            url: 'api/Search/Search',
            data: {
                skip: self.results().length,
                take: self.pageSize
            }
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