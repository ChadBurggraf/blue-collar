/**
 * Pager model.
 */
var Pager = Backbone.Model.extend({
    parse: function(response) {
        response = response || {};

        return {
            PageCount: response.pageCount || 1,
            PageNumber: response.pageNumber || 1,
            TotalCount: response.totalCount || 0
        };
    }
});