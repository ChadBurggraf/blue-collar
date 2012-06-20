/**
 * Base collection implementation.
 *
 * @constructor
 */
 var CollarCollection = Backbone.Collection.extend({
    /**
     * Initialization.
     *
     * @param {Object} models An object specifying the model collection.
     * @param {Object} options Initialization options.
     */
    initialize: function(models, options) {
        options = options || {};
        this.search = '';
        this.urlRoot = options.urlRoot || '/';

        // Reset is called by the true Backbone.Collection constructor
        // if models is defined. Therefore, only call if models is
        // not defined.
        if (!models) {
            this.reset(models, {silent: true});
        }
    },

    /**
     * Clears the 'editing' attribute from each model in the collection.
     */
    clearEditing: function() {
        this.each(function(m) { 
            m.set({editing: false}); 
        });
    },

    /**
     * Creates a params object for listing the collection.
     */
    listParams: function() {
        return {
            search: this.search || '',
            page: this.pageNumber || 1
        };
    },

    /**
     * Replaces this instance's model collection with the given collection.
     *
     * @param {Object} models An object specifying the new model collection.
     * @param {Object} options The options to use when performing the reset.
     * @return {CollarCollection} This instance.
     */
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

    /**
     * Gets the URL to use when interacting with the collection on the server.
     *
     * @return {String} The collection's server URL.
     */
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