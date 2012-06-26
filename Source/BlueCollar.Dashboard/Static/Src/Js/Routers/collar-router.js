/**
 * Base router implementation.
 *
 * @constructor
 */
var CollarRouter = Backbone.Router.extend({
    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        this.app = app;
        this.options = _.extend({}, options);
    },

    /**
     * Handles controller-initiated navigate events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    controllerNavigate: function(sender, args) {
        var url;

        args = _.extend({
            Fragment: '',
            Search: '',
            PageNumber: 1
        }, args);

        url = args.Fragment;

        if (args.Search || args.PageNumber > 1) {
            url += '/' + encodeURIComponent(args.Search || '');

            if (args.PageNumber > 1) {
                url += '/p' + encodeURIComponent(args.PageNumber.toString());
            }
        }

        this.navigate(url);
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
     * Creates a new instance of this router's default controller.
     *
     * @param {Function} func The constructor function of the controller to create.
     * @param {String} fragment The URL-root the controller uses to interact with the server.
     * @param {Object} options Initialization options to use when creating the controller.
     * @return The created controller.
     */
    createController: function(func, fragment, options) {
        var controller = new func(
            this.app.name,
            this.app.jsonUrlRoot + fragment,
            this.app.page,
            options);

        controller.bind('counts', this.counts, this);
        controller.bind('navigate', this.controllerNavigate, this);
        return controller;
    }
});