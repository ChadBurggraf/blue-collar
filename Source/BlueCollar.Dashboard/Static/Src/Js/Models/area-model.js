/**
 * Models an application area, consisting of list and details panes.
 *
 * @constructor
 */
var AreaModel = Backbone.Model.extend({
    defaults: {
        ApplicationName: 'Default',
        Collection: new CollarCollection(),
        Id: 0,
        Loading: false,
        PageCount: 0,
        PageNumber: 1,
        Search: '',
        TotalCount: 0
    },

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        var collection = this.get('Collection');

        this.bind('change:Id', this.id, this);
        this.bind('change:UrlRoot', this.changeUrlRoot, this);

        if (collection) {
            collection.bind('area', this.area, this);
            collection.bind('reset', this.reset, this);
        }
    },

    /**
     * Handles the collection's area event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    area: function(sender, args) {
        this.set(_.extend({}, args, {
            Loading: false
        }));
    },

    /**
     * Handles this isntance's UrlRoot-change event.
     */
    changeUrlRoot: function() {
        var collection = this.get('Collection');

        if (collection) {
            if (_.isFunction(collection.setUrlRoot)) {
                collection.setUrlRoot(this.get('UrlRoot'));
            } else {
                collection.urlRoot = this.get('UrlRoot');
            }
        }
    },

    /**
     * Clears this instance's selected ID.
     */
    clearId: function(options) {
        this.set({Id: 0, Action: ''}, options);
    },

    /**
     * Handles this instance's ID-change event.
     */
    id: function() {
        var collection = this.get('Collection');

        if (collection) {
            collection.setSelected(this.get('Id'));
        }
    },

    /**
     * Handles this instance's collection's reset event.
     */
    reset: function() {
        var collection = this.get('Collection');

        if (collection) {
            collection.setSelected(this.get('Id'));
        }
    }
});