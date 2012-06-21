/**
 * Base form view implementation.
 *
 * @constructor
 */
var FormView = Backbone.View.extend({
    className: 'form well',
    tagName: 'form',
    inputSelector: 'input, select, textarea',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.options = _.extend({
            errorClassName: 'error',
            fieldSelector: '.field',
            validationSummaryMessage: 'Please correct the errors below.',
            validationSummarySelector: '.alert.alert-block.error'
        }, this.options);

        this.$el.attr('action', 'javascript:void(0);');
        this.isLoading = false;
    },

    /**
     * Handles the non-delete cancel button press.
     */
    cancel: function() {
        this.trigger('cancel', this);
    },

    /**
     * Handles the cancel delete button press.
     */
    cancelDelete: function() {
        this.$('.form-actions')
            .hide()
            .filter(':not(.form-actions-delete)')
            .fadeIn();
    },

    /**
     * Handles the confirm-delete button press.
     */
    confirmDelete: function() {
        this.trigger('delete', this);
    },

    /**
     * Handles an initial (pre-confirmation) delete link click.
     */
    del: function() {
        this.$('.form-actions')
            .hide()
            .filter('.form-actions-delete')
            .fadeIn();
    },

    /**
     * Gets a jQuery object containing all of the form's fields.
     */
    findFields: function(name) {
        return this.$('input[name="' + name + '"], textarea[name="' + name + '"], select[name="' + name + '"]');
    },

    /**
     * Focuses the first element in the form.
     */
    focus: function() {},

    /**
     * Updates the view to reflect that submitting data to or loading
     * data from the server has completed.
     */
    hideLoading: function() {
        var elements,
            el,
            data,
            i,
            n;

        if (this.isLoading) {
            elements = this.$(this.inputSelector);

            for (i = 0, n = elements.length; i < n; i++) {
                el = $(elements[i]);
                data = el.data('FormView:Loading');

                if (data) {
                    el[0].disabled = data.disabled;
                }

                el.removeData('FormView:Loading');
            }

            this.isLoading = false;
        }
    },

    /**
     * Renders the view.
     *
     * @return {FormView} This instance.
     */
    render: function() {
        var attributes = !_.isUndefined(this.model.toEditJSON) ? this.model.toEditJSON() : this.model.toJSON(),
            actions,
            actionsDelete,
            del;

        this.$el.html(this.template(attributes));
        this.deserialize(attributes);
        this.renderErrors();

        actions = this.$('.form-actions:not(.form-actions-delete)').show();
        actionsDelete = actions.find('a.delete');
        del = this.$('.form-actions-delete').hide();

        this.$el.bind('submit', _.bind(this.submit, this));
        actions.find('button[type="reset"]').bind('click', _.bind(this.cancel, this));

        if (this.model.get('Id')) {
            actionsDelete.bind('click', _.bind(this.del, this));
            del.find('button.btn-danger').bind('click', _.bind(this.confirmDelete, this));
            del.find('button:not(.btn-danger)').bind('click', _.bind(this.cancelDelete, this));
        } else {
            actionsDelete.remove();
            del.remove();
        }

        return this;
    },

    /**
     * Updates the view to reflect submitting data to or loading data from
     * the server.
     */
    showLoading: function() {
        var elements,
            el,
            i,
            n;

        if (!this.isLoading) {
            this.isLoading = true;
            elements = this.$(this.inputSelector);

            for (i = 0, n = elements.length; i < n; i++) {
                el = $(elements[i]);
                el.data('FormView:Loading', {disabled: el[0].disabled});
                el.attr('disabled', 'disabled');
            }
        }   
    }
});