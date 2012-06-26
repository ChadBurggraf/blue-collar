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
     * Performs a fetch opteration on this collection.
     *
     * @param {Object} options The fetch options to use.
     * @return {jqXHR} The XHR object used to perform the fetch.
     */
    fetch: function(options) {
        return Backbone.Collection.prototype.fetch.call(this, _.extend({url: this.url(options)}, options));
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
        options = options || {};
        
        if (!options.silent) {
            if (models.PageCount || models.PageNumber || models.TotalCount) {
                this.trigger('area', this, {PageCount: models.PageCount, PageNumber: models.PageNumber, TotalCount: models.TotalCount});
            }
        
            if (models.Counts) {
                this.trigger('counts', this, {Counts: models.Counts});
            }
        }

        return Backbone.Collection.prototype.reset.call(this, models.Records, options);
    },

    /**
     * Gets the URL to use when interacting with the collection on the server.
     *
     * @param {Object} options The options to use when generating the URL.
     * @return {String} The collection's server URL.
     */
    url: function(options) {
        var url = this.urlRoot || '/',
            queryIndex = url.indexOf('?');

        options = _.extend({
            pageNumber: 1,
            search: ''
        }, options);

        if (queryIndex < 0) {
            url += '?';
        } else if (queryIndex !== url.length && url.lastIndexof('&') !== url.length) {
            url += '&';
        }

        url += 'q=' + encodeURIComponent(options.search || '');
        url += '&p=' + encodeURIComponent((options.pageNumber || 1).toString());

        return url;
    }
 });