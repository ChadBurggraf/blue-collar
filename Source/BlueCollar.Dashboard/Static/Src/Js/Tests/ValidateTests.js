module('Validate');

test('ValidateRange', function() {
    equal(new RangeValidator({min: 0, max: 1}).validate(1), undefined);
    equal(new RangeValidator({min: 1, max: 2}).validate(1), undefined);
    equal(new RangeValidator({min: 0, max: 2}).validate(0), undefined);
    notEqual(new RangeValidator({min: 3, max: 4}).validate(2), undefined);
    equal(new RangeValidator({min: 0, max: 100}).validate(null), undefined);
    equal(new RangeValidator({min: Date.parse('2010-01-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
    notEqual(new RangeValidator({min: Date.parse('2011-02-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
});

test('ValidateRegex', function() {
    equal(new RegexValidator({exp: /^abc$/}).validate('abc'), undefined);
    equal(new RegexValidator({exp: '^abc$'}).validate('abc'), undefined);
    notEqual(new RegexValidator({exp: /^def$/}).validate('abc'), undefined);
    equal(new RegexValidator({exp: /^abc$/}).validate(''), undefined);
});

test('ValidateRequired', function() {
    equal(new RequiredValidator().validate('a'), undefined);
    notEqual(new RequiredValidator().validate(''), undefined);
    equal(new RequiredValidator().validate(0), undefined);
    equal(new RequiredValidator().validate(12), undefined);
    notEqual(new RequiredValidator().validate(NaN), undefined);
    equal(new RequiredValidator().validate(new Date()), undefined);
    equal(new RequiredValidator().validate([1, 2, 3]), undefined);
    notEqual(new RequiredValidator().validate([]), undefined);
    notEqual(new RequiredValidator().validate(undefined), undefined);
    notEqual(new RequiredValidator().validate(null), undefined);
    notEqual(new RequiredValidator().validate({}), undefined);
});