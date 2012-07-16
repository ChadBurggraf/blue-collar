/**
 * Models an application area, consisting of list and details panes.
 *
 * @constructor
 */
var AreaModel = CollarModel.extend({
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
     * @param {Object} app A set of initial model attribute values.
     * @param {Object} options Initialization options.
     */
    initialize: function(attributes, options) {
        var collection;
        
        

        this.bind('change:Id', this.id, this);
        collection = this.get('Collection');

        if (collection) {
            collection.bind('area', this.area, this);
            collection.bind('reset', this.reset, this);
        }

        CollarModel.prototype.initialize.call(this, attributes, options);
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
     * Clears this instance's selected ID.
     */
    clearId: function(options) {
        this.set({Id: 0, Action: ''}, options);
    },

    /**
     * Handles this instance's ID-change event.
     */
    id: function() {
        debugger;
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