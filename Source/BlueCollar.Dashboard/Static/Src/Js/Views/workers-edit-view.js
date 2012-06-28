/**
 * Implements the workers edit form.
 *
 * @constructor
 * @extends {FormView}
 */
var WorkersEditView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer(),
        "QueueNames": new QueueNamesSerializer()
    },
    template: _.template($('#workers-edit-template').html()),
    validators: {
        "Name": [
            new RequiredFieldValidator({message: 'Name is required.'}),
            new LengthFieldValidator({maxLength: 64, message: 'Name cannot be longer than 64 characters.'})
        ],
        "MachineAddress": [
            new LengthFieldValidator({maxLength: 64, message: 'Machine address cannot be longer than 64 characters.'})
        ],
        "MachineName": [
            new LengthFieldValidator({maxLength: 128, message: 'Machine name cannot be longer than 128 characters.'})
        ],
        "Startup": [
            new RequiredFieldValidator({message: 'Startup is required.'}),
            new EnumFieldValidator({possibleValues: ['Automatic', 'Manual'], message: 'Startup must be either Automatic or Manual.'})
        ]
    },

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);
        this.machines = this.options.machines || [];

        this.events = _.extend(this.events, {
            'click .field-choose a': 'choose',
            'click .field-enter a': 'enter'
        });

        this.delegateEvents();
    },

    /**
     * Handles the choose link click event.
     */
    choose: function() {
        this.$('.field-choose').hide();
        this.$('.field-enter').show().find('input[name="MachineName"]').focus();
    },

    /**
     * De-serializes the given attributes hash into this view's form fields.
     *
     * @param {Object} attributes A hash of attribute values to fill this instance with.
     * @return {FormView} This instance.
     */
    deserialize: function(attributes) {
        FormView.prototype.deserialize.call(this, attributes);
        this.deserializeMachineSelect();

        if (this.machines.length > 0) {
            this.$('.field-choose').show();
            this.$('.field-enter').hide();
        } else {
            this.$('.field-choose').remove();
            this.$('.field-enter a').remove();
        }

        return this;
    },

    /**
     * De-serializes the machine select input.
     *
     * @return {WorkerEditView} This instance.
     */
    deserializeMachineSelect: function() {
        var modelMachine = this.machineOptionValue(this.model.machine()),
            chooseSelect = this.$('.field-choose select')[0],
            enter = this.$('.field-enter'),
            text,
            value,
            i,
            n,
            s = 0;

        for (i = 0, n = this.machines.length; i < n; i++) {
            text = this.machineOptionText(this.machines[i]);
            value = this.machineOptionValue(this.machines[i]);
            chooseSelect.options[i] = new Option(text, value);

            if (value === modelMachine) {
                s = i;
            }
        }

        chooseSelect.selectedIndex = s;
        return this;
    },

    /**
     * Handles the enter link click event.
     */
    enter: function() {
        this.$('.field-choose').show();
        this.$('.field-enter').hide();
    },

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('input[name="Name"]').focus();
        return this;
    },

    /**
     * Gets a machine object from the given option value.
     *
     * @param {String} value The option value describing the machine.
     * @return {Object} A machine object, or null if none was found.
     */
    machineFromOptionValue: function(value) {
        var name,
            address,
            i,
            n;

        value = _.map($.splitAndTrim(decodeURIComponent(value || ''), '&'), function(s) { return decodeURIComponent(s); });
        
        if (value.length === 2) {
            name = _.find(value, function(s) { return s.indexOf('n=') === 0; });
            name = name ? name.substr(2) : '';

            address = _.find(value, function(s) { return s.indexOf('a=') === 0; });
            address = address ? address.substr(2) : '';
            
            for (i = 0, n = this.machines.length; i < n; i++) {
                if (this.machines[i].Name === name && this.machines[i].Address === address) {
                    return this.machines[i];
                }
            }
        }

        return null;
    },

    /**
     * Gets option display text for the given machine object.
     *
     * @param {Object} machine The machine object to get option display text for.
     * @return {String} Option display text.
     */
    machineOptionText: function(machine) {
        var t = '';

        if (machine.Name) {
            t = machine.Name;
        };

        if (machine.Address) {
            if (machine.Name) {
                t += ' (';
            }

            t += machine.Address;

            if (machine.Name) {
                t += ')';
            }
        }

        return t;
    },

    /**
     * Gets an option value string for the given machine object.
     *
     * @param {Object} machine The machine object to get an option value string for.
     * @return {String} An option value string.
     */
    machineOptionValue: function(machine) {
        return encodeURIComponent('n=' + encodeURIComponent(machine.Name) + '&a=' + encodeURIComponent(machine.Address));
    },

    /**
     * Serializes the form.
     *
     * @return {Object} The serialized form attributes.
     */
    serialize: function() {
        var obj = FormView.prototype.serialize.call(this),
            machine = null;

        if (this.machines.length > 0 && this.$('.field-choose').is(':visible')) {
            machine = this.machineFromOptionValue(this.$('.field-choose select').val());
        }
        
        if (!machine && this.$('.field-enter').is(':visible')) {
            machine = {
                Name: this.$('.field-enter input[name="MachineName"]').val(), 
                Address: this.$('.enter input[name="MachineAddress"]').val()
            };
        }

        obj.MachineName = machine.Name;
        obj.MachineAddress = machine.Address;
        debugger;
        return obj;
    }
});