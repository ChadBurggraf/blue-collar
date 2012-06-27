/**
 * Schedules area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var SchedulesRouter = CollarRouter.extend({
    name: 'Schedules',
    routes: {
        'schedules': 'index',
        'schedules/:search/p:page': 'index',
        'schedules//p:page': 'page',
        'schedules/*search': 'search'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(SchedulesController, 'schedules', this.options);
    }
});