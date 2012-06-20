/**
 * Working area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var WorkingRouter = CollarRouter.extend({
    routes: {
        'working': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Additional initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.options = _.extend({}, options);
    },

    /**
     * Handles the root #working route.
     */
    index: function() {
        
    }
});