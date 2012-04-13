/**
 * Base extension of Backbone.Collection for Blue Collar collections.
 * Provides default model unpacking and URL behaviors.
 */
var BlueCollarCollection = Backbone.Collection.extend({
    initialize: function(models, options) {
        options = options || {};
        this.reset(models, {silent: true});
        this.search = '';
        this.urlRoot = options.urlRoot || '/';
    },

    clearEditing: function() {
        this.each(function(m) { m.set({editing: false}); });
    },

    listParams: function(force) {
        return {
            search: this.search || '',
            page: this.pageNumber || 1,
            force: !!force
        };
    },

    reset: function(models, options) {
        models = models || {};
        this.pageCount = models.PageCount || 1;
        this.pageNumber = models.PageNumber || 1;
        this.totalCount = models.TotalCount || 0;
        
        if (models.Counts) {
            this.trigger('counts', this, models.Counts);
        }

        return Backbone.Collection.prototype.reset.call(this, models.Records, options);
    },

    url: function() {
        var url = this.urlRoot || '/',
            queryIndex = url.indexOf('?');

        if (queryIndex < 0) {
            url += '?';
        } else if (queryIndex !== url.length && url.lastIndexof('&') !== url.length) {
            url += '&';
        }

        url += 'q=' + encodeURIComponent(this.search || '');
        url += '&p=' + encodeURIComponent((this.pageNumber || 1).toString());

        return url;
    }
});