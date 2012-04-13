/** 
 * Details view for Queued models.
 */
var QueuedDetailsView = FormView.extend({
    template: _.template($('#queued-details-template').html()),

    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);
        this.model.bind('change', this.render, this);
    },

    render: function() {
        FormView.prototype.render.call(this);
        setTimeout(prettyPrint, 100);
        return this;
    }
});