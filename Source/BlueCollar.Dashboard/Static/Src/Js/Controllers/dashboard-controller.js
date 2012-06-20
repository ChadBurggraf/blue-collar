/**
 * Dashboard area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var DashboardController = CollarController.extend({
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.view = new DashboardView({el: this.page});
    },

    index: function(options) {
        this.view.render();
    }
});