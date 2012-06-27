/**
 * Manages the root working jobs view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkingView = AreaView.extend({
    template: _.template($('#working-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.listView = new WorkingListView({model: this.model});
        this.listView.bind('display', this.display, this);
    },

    /**
     * Handles the list view's display event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    display: function(sender, args) {

    }
});