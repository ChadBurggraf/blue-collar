/**
 * Base controller implementation.
 *
 * @constructor
 * @param {String} applicationName The name of the application.
 * @param {String} urlRoot The JSON URL root the application is using.
 * @param {jQuery} page A reference to the page jQuery element.
 * @param {Object} options Initialization options. 
 */
var CollarController = function(applicationName, urlRoot, page, options) {
    this.applicationName = applicationName;
    this.urlRoot = urlRoot;
    this.page = page;
    this.options = _.extend({}, options);

    var collection = null;

    if (_.isFunction(this.collection)) {
        collection = new this.collection(null, {urlRoot: this.urlRoot});
        collection.bind('reset', this.reset, this);
    }

    this.model = new Backbone.Model({ApplicationName: this.applicationName, Collection: collection});
    this.initialize(this.options);
};

/**
 * Static functions.
 */
_.extend(CollarController, {
    /**
     * Mixin extend functionality to enable inheritence.
     */
    extend: extend
});

/**
 * Prototype functions.
 */
_.extend(CollarController.prototype, Backbone.Events, {
    collection: CollarCollection,
    fragment: '',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {},

    /**
     * Generic handler for Ajax-related errors.
     *
     * @param {Backbone.View} view The view the error is related to.
     * @param {Backbone.Model} model The model the error is related to.
     * @param {jqXHR} response The response received from the server.
     */
    ajaxError: function(view, model, response) {
        var collection = this.getCollection();

        if (view && _.isFunction(view.hideLoading)) {
            view.hideLoading();
        }

        if (collection) {
            collection.each(function(m) { 
                m.set({editing: false}); 
            });
        }

        if (!view || !view.ajaxError(model, response)) {
            var message;

            switch (response.status) {
                case 400:
                    message = 'Bad Request (400): The server indicated that you submitted was invalid or impropertly formatted.';
                    break;
                case 403:
                    message = 'Forbidden (403): You are not authorized to access the requested resource.';
                    break;
                case 404:
                    message = 'Not Found (404): The requested resource was not found on the server.';
                    break;
                case 500:
                    message = 'Internal Server Error (500): Something when wrong when processing your request on the server.';
                    break;
                default:
                    message = 'Unknown Error (' + response.status + '): An unknown error occurred while processing your request on the server.';
                    break;
            }

            NoticeView.create({
                className: 'error',
                model: {Title: 'Uh Oh, That Kinda Hurt', Message: message}
            });
        }
    },

    /**
     * Handles counts update events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    counts: function(sender, args) {
        this.trigger('counts', this, args);
    },

    /**
     * Gets this instance's current collection instance, if applicable.
     *
     * @return {CollarCollection} The current collection instance, or null if none exists.
     */
    getCollection: function() {
        var collection = this.model && _.isFunction(this.model.get) ? this.model.get('Collection') : null;
        return _.isFunction(collection) ? collection : null;
    },

    /**
     * Performs navigation on behalf of this controller.
     */
    navigate: function() {
        var collection = this.getCollection(),
            fragment = this.navigateFragment(),
            search,
            page;

        if (collection) {
            search = collection.search || '';
            page = collection.pageNumber || 1;
        }

        this.trigger('navigate', this, {fragment: fragment, search: search, page: page});
    },

    /**
     * Gets the URL fragment to use when navigating.
     *
     * @return {String} A URL fragment.
     */
    navigateFragment: function() {
        return this.fragment || '';
    },

    /**
     * Handles reset events sent to this instance.
     */
    reset: function() {
        var collection = this.getCollection();

        if (collection && collection.length === 0 && collection.pageNumber > 1) {
            collection.pageNumber = 1;
            this.navigate();
        }
    }
});