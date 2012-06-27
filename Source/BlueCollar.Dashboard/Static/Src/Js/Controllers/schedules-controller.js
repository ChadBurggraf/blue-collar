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
    }
});