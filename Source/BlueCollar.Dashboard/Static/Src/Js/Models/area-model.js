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
        this.bind('change:Id', this.id, this);
        this.get('Collection').bind('area', this.area, this);
        this.get('Collection').bind('reset', this.reset, this);
    },

    /**
     * Handles the collection's area event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    area: function(sender, args) {
        this.set({
            Loading: false,
            PageCount: args.PageCount,
            PageNumber: args.PageNumber,
            TotalCount: args.TotalCount
        });
    },

    /**
     * Handles this instance's ID-change event.
     */
    id: function() {
        this.get('Collection').setSelected(this.get('Id'));
    },

    /**
     * Handles this instance's collection's reset event.
     */
    reset: function() {
        this.get('Collection').setSelected(this.get('Id'));
    }
});