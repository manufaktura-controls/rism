var SearchViewModel = function () {
    var self = this;
    this.pageSize = 30;
    this.results = ko.observableArray([]);
    this.player = new PlaybackManager();

    this.getMoreResults = function () {
        $.ajax({
            type: "GET",
            url: 'api/Search/Search',
            data: {
                skip: self.results().length,
                take: self.pageSize
            }
        }).done(function (response) {
            console.info(response);
            for (var i in response) {
                var result = response[i];
                self.results.push(result);
            }
        });
    }
}