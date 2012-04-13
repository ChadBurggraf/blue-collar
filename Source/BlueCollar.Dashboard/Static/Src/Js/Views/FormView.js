/**
 * Base view implementation for forms. Provides
 * default rendering, validation and serialization.
 */
var FormView = Backbone.View.extend({
    className: 'form-stacked',
    tagName: 'form',
    inputSelector: 'input, select, textarea',

    initialize: function(options) {
        this.options = _.extend({
            errorClassName: 'error',
            fieldSelector: '.clearfix',
            validationSummaryMessage: 'Please correct the errors below.',
            validationSummarySelector: '.alert-message.block-message.error'
        }, this.options);

        $(this.el).attr('action', 'javascript:void(0);');
        this.isLoading = false;
    },

    ajaxError: function(model, response) {
        if (/^application\/json/i.test(response.getResponseHeader('Content-Type'))) {
            var json = null;

            try {
                json = JSON.parse(response.responseText);
            } catch (e) {
            }

            if (json && !_.isEmpty(json)) {
                this.renderErrors(json);
                return true;
            }
        }

        return false;
    },

    cancel: function() {
        this.trigger('cancel', this);
    },

    cancelDelete: function() {
        this.$('.actions')
            .hide()
            .filter(':not(.delete)')
            .fadeIn();
    },

    confirmDelete: function() {
        this.trigger('delete', this);
    },

    del: function() {
        this.$('.actions')
            .hide()
            .filter('.delete')
            .fadeIn();
    },

    deserialize: function(attributes) {
        new FormSerializer().deserialize($(this.el), attributes, this.serializers);
        return this;
    },

    findFields: function(name) {
        return this.$('input[name="' + name + '"], textarea[name="' + name + '"], select[name="' + name + '"]');
    },

    focus: function() {},

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

    render: function() {
        var el = $(this.el),
            attributes = !_.isUndefined(this.model.toEditJSON) ? this.model.toEditJSON() : this.model.toJSON(),
            actions,
            actionsDelete,
            del;

        el.html(this.template(attributes));
        this.deserialize(attributes);
        this.renderErrors();

        actions = this.$('.actions:not(.delete)').show();
        actionsDelete = actions.find('a.delete');
        del = this.$('.actions.delete').hide();

        el.bind('submit', _.bind(this.submit, this));
        actions.find('button[type="reset"]').bind('click', _.bind(this.cancel, this));

        if (this.model.get('Id')) {
            actionsDelete.bind('click', _.bind(this.del, this));
            del.find('button.danger').bind('click', _.bind(this.confirmDelete, this));
            del.find('button:not(.danger)').bind('click', _.bind(this.cancelDelete, this));
        } else {
            actionsDelete.remove();
            del.remove();
        }

        return this;
    },

    renderErrors: function(errors) {
        var summary = this.$(this.options.validationSummarySelector),
            fields = this.$(this.options.fieldSelector),
            summaryErrors = [],
            summaryList,
            prop,
            error,
            inputs,
            field,
            errorEl,
            found;

        summary.hide().html('');

        fields
            .removeClass(this.options.errorClassName)
            .find('.' + this.options.errorClassName)
            .hide();

        errors = errors || {};

        for (prop in errors) {
            if (errors.hasOwnProperty(prop)) {
                error = (errors[prop] || '').toString();

                if (error) {
                    found = false;
                    inputs = this.findFields(prop);

                    if (inputs.length > 0) {
                        field = inputs.parents(this.options.fieldSelector);
                        errorEl = field.find('.' + this.options.errorClassName);

                        if (field.length > 0 && errorEl.length > 0) {
                            field.addClass(this.options.errorClassName);
                            errorEl.text(error).show();

                            found = true;
                        }
                    }

                    if (!found) {
                        summaryErrors.push(error);
                    }
                }
            }
        }

        if (summaryErrors.length > 0) {
            summaryList = $('<ul/>');
            summaryList.append.apply(summaryList, _.map(summaryErrors, function(e) { return $('<li/>').text(e); }));

            summary
                .append($('<p/>').text(this.options.validationSummaryMessage))
                .append(summaryList)
                .show();
        }

        return this;
    },

    serialize: function() {
        var obj = new FormSerializer().serialize(
            $(this.el), 
            !_.isUndefined(this.model.toEditJSON) ? this.model.toEditJSON() : this.model.toJSON(),
            this.serializers);

        return obj;
    },

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
    },

    submit: function() {
        var attributes = this.serialize(),
            errors = this.validate(attributes);

        this.renderErrors(errors);

        if (!errors) {
            this.trigger('submit', this, attributes);
        }

        return this;
    },

    validate: function(attributes) {
        var errors = {},
            prop,
            validators,
            message,
            i,
            n

        attributes = attributes || {};

        if (!_.isUndefined(this.validators) && !_.isNull(this.validators)) {
            for (prop in this.validators) {
                if (this.validators.hasOwnProperty(prop)) {
                    validators = this.validators[prop];

                    if (!_.isArray(validators)) {
                        validators = [validators];
                    }

                    for (i = 0, n = validators.length; i < n; i++) {
                        message = validators[i].validate(attributes[prop]);

                         if (message) {
                            errors[prop] = message;
                        }
                    }
                }
            }
        }

        if (!_.isEmpty(errors)) {
            return errors;
        }
    }
});