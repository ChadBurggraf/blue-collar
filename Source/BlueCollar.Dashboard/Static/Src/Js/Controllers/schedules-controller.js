/**
 * Schedules area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var SchedulesController = CollarController.extend({
    collection: ScheduleCollection,
    fragment: 'schedules',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.view = new SchedulesView({el: this.page, model: this.model});
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('editDelete', this.editDelete, this);
        this.view.bind('editSubmit', this.editSubmit, this);
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        var model = this.model.get('Collection').find(function(m) { return m.get('Id') === args.Model.get('Id'); });
        CollarController.prototype.success.call(this, args, model, response);

        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The schedule ' + model.get('Name') + ' was ' + args.Action + ' successfully.'}
        });
    }
});