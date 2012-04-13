/**
 * FormView for Worker models.
 */
var WorkerFormView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer(),
        "QueueNames": new QueueNamesSerializer()
    },

    template: _.template($('#worker-form-template').html()),

    validators: {
        "Name": [
            new RequiredValidator({message: 'Name is required.'}),
            new LengthValidator({maxLength: 64, message: 'Name cannot be longer than 64 characters.'})
        ],
        "MachineAddress": [
            new LengthValidator({maxLength: 64, message: 'Machine address cannot be longer than 64 characters.'})
        ],
        "MachineName": [
            new LengthValidator({maxLength: 128, message: 'Machine name cannot be longer than 128 characters.'})
        ],
        "Startup": [
            new RequiredValidator({message: 'Startup is required.'}),
            new EnumValidator({possibleValues: ['Automatic', 'Manual'], message: 'Startup must be either Automatic or Manual.'})
        ]
    },

    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);
        this.machines = options.machines || [];
    },

    choose: function() {
        var chooseSelect = this.$('.choose select')[0],
            modelValue = this.machineOptionValue({Name: this.model.get('MachineName') || '', Address: this.model.get('MachineAddress') || ''}),
            selectedIndex = 0,
            i,
            n;

        for (i = 0, n = chooseSelect.options.length; i < n; i++) {
            if (chooseSelect.options[i].value === modelValue) {
                selectedIndex = i;
                break;
            }
        }

        chooseSelect.selectedIndex = selectedIndex;

        this.$('.enter').hide();
        this.$('.choose').show();
        chooseSelect.focus();
    },

    enter: function() {
        this.$('.choose').hide();
        this.$('.enter').show();
        this.$('.enter input[name="MachineName"]').val(this.model.get('MachineName')).focus();
        this.$('.enter input[name="MachineAddress"]').val(this.model.get('MachineAddress'));
    },

    focus: function() {
        this.$('input[name="Name"]').focus();
    },

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

    machineOptionValue: function(machine) {
        return encodeURIComponent('n=' + encodeURIComponent(machine.Name) + '&a=' + encodeURIComponent(machine.Address));
    },

    render: function() {
        var chooseSelect,
            text,
            value,
            modelValue,
            selectedIndex = 0,
            i,
            n;

        FormView.prototype.render.call(this);

        if (this.machines.length > 0) {
            modelValue = this.machineOptionValue({Name: this.model.get('MachineName') || '', Address: this.model.get('MachineAddress') || ''});

            chooseSelect = this.$('.choose select')[0];
            chooseSelect.options.length = 0;

            for (i = 0, n = this.machines.length; i < n; i++) {
                text = this.machineOptionText(this.machines[i]);
                value = this.machineOptionValue(this.machines[i]);
                chooseSelect.options[i] = new Option(text, value);

                if (value === modelValue) {
                    selectedIndex = i;
                }
            }

            chooseSelect.selectedIndex = selectedIndex;

            this.$('.choose a').show().click(_.bind(this.enter, this));
            this.$('.enter a').show().click(_.bind(this.choose, this));
            this.$('.choose').show();
            this.$('.enter').hide();
        } else {
            this.$('.choose a, .enter a').hide();
            this.$('.choose').hide();
            this.$('.enter').show();

            this.$('.enter input[name="MachineName"]').val(this.model.get('MachineName'));
            this.$('.enter input[name="MachineAddress"]').val(this.model.get('MachineAddress'));
        }

        return this;
    },

    serialize: function() {
        var obj = FormView.prototype.serialize.call(this),
            machine = null;

        if (this.machines.length > 0 && this.$('.choose').is(':visible')) {
            machine = this.machineFromOptionValue(this.$('.choose select').val());
        }
        
        if (!machine && this.$('.enter').is(':visible')) {
            machine = {Name: this.$('.enter input[name="MachineName"]').val(), Address: this.$('.enter input[name="MachineAddress"]').val()};
        }

        obj.MachineName = machine.Name;
        obj.MachineAddress = machine.Address;

        return obj;
    },

    validate: function(attributes) {
        var errors = FormView.prototype.validate.call(this, attributes),
            machine;

        if (!attributes['MachineName'] && !attributes['MachineAddress']) {
            machine = {'Machine': 'Machine name, machine address, or both must be specified.'};

            if (_.isUndefined(errors)) {
                errors = machine;
            } else {
                _.extend(errors, machine);
            }
        }

        return errors;
    }
});