/**
 * Manages the root queue view.
 *
 * @constructor
 * @extends {AreaView}
 */
var QueueView = AreaView.extend({
    template: _.template($('#queue-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);

        this.model.get('Collection').bind('reset', this.renderId, this);

        this.listView = new QueueListView({model: this.model});
        this.listView.bind('display', this.display, this);
        this.listView.bind('signal', this.signal, this);
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {
        var render = false,
            view,
            signalModel;

        if (this.model.get('Action') === 'signal') {
            if (model.get('Signal') === 'None') {

            } else {
                this.model.set({Id: 0, Action: ''});
            }
        } else {
            view = new QueueDisplayView({model: model});
            render = true;
        }

        if (render) {
            el.html(view.render().el);
            view.focus();
        }
    }
});