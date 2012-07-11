/**
 * Implements the working display form.
 *
 * @constructor
 * @extends {FormView}
 */
var WorkingDisplayView = FormView.extend({
    template: _.template($('#working-display-template').html()),

    /**
     * Handles model change events.
     */
    change: function() {
        this.render();
    },

    /**
     * Renders the view.
     *
     * @return {FormView} This instance.
     */
    render: function() {
        FormView.prototype.render.call(this);
        setTimeout(prettyPrint, 100);
        return this;
    }
});