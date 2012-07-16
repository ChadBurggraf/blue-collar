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

        if (!this.fragment) {
            this.fragment = options.fragment || '';
        }

        if (!this.jsonUrlRoot) {
            this.jsonUrlRoot = options.jsonUrlRoot || '';
        }

        // Reset is called by the true Backbone.Collection constructor
        // if models is defined. Therefore, only call if models is
        // not defined.
        if (!models) {
            this.reset(models, {silent: true});
        }
    },

    /**
     * Clears the 'Selected' attribute from each model in the collection.
     *
     * @param {Object} options The set options to use.
     * @return {CollarCollection} This instance.
     */
    clearSelected: function(options) {
        options = options || {};

        this.each(function(m) { 
            m.set({Selected: false}, options); 
        });

        return this;
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
     * Gets the selected model.
     *
     * @return {CollarModel} The selected model, or undefined if none are selected.
     */
    getSelected: function() {
        return this.find(function(m) {
            return m.get('Selected');
        });
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
        options = _.extend({fragment: this.fragment}, options);
        
        if (!options.silent) {
            this.triggerArea(models);
            this.triggerCounts(models);
        }

        Backbone.Collection.prototype.reset.call(this, models.Records, options);
        return this;
    },

    /**
     * Sets the selected model by ID.
     *
     * @param {Number} id The ID of the model to set as selected.
     * @param {Object} options The set options to use.
     * @return {CollarCollection} This instance.
     */
    setSelected: function(id, options) {
        var model,
            i,
            n;

        options = options || {};
        
        for (i = 0, n = this.length; i < n; i++) {
            model = this.at(i);

            if (model.get('Id') === id) {
                model.set({Selected: true}, options);
            } else {
                model.set({Selected: false}, options);
            }
        }

        return this;
    },

    /**
     * Sets this instance's urlRoot property.
     *
     * @param {String} urlRoot The value to set.
     */
    setUrlRoot: function(urlRoot) {
        this.urlRoot = urlRoot;
    },

    /**
     * Triggers the area event for this instance, if the givem models object area information.
     *
     * @param {Object} models The models object being used to reset this instance.
     */
    triggerArea: function(models) {
        if (models.PageCount || models.PageNumber || models.TotalCount) {
            this.trigger('area', this, {PageCount: models.PageCount, PageNumber: models.PageNumber, TotalCount: models.TotalCount});
        }
    },

    /**
     * Triggers the counts event for this instance, if the givem models object contains a counts object.
     *
     * @param {Object} models The models object being used to reset this instance.
     */
    triggerCounts: function(models) {
        if (models.Counts) {
            this.trigger('counts', this, {Counts: models.Counts});
        }
    },

    /**
     * Gets the URL to use when interacting with the collection on the server.
     *
     * @param {Object} options The options to use when generating the URL.
     * @return {String} The collection's server URL.
     */
    url: function(options) {
        var url = _.isFunction(this.urlRoot) ? this.urlRoot() : (this.urlRoot || '/'),
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
    },

    /**
     * Gets the URL root to use when interacting with the collection on the server.
     *
     * @return {String} The collection's server URL root.
     */
    urlRoot: function() {
        return (this.jsonUrlRoot || '/').appendUrlPath(this.fragment);
    }
 });