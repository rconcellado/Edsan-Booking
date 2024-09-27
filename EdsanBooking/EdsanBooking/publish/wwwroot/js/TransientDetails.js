$(document).ready(function () {
    $('#addTransientDetailToReservation').on('click', function () {
        var featureName = $('#featureName').val();
        var typeName = $('#typeName').val();
        var hourType = $('#hourType').val();

        // Create a unique identifier for this detail
        var detailId = 'transient-detail-' + featureName + '-' + typeName + '-' + hourType;

        // Create list item for displaying added details
        var detailItem = '<li class="list-group-item" id="' + detailId + '-item">' + featureName + ' - ' + typeName + ' - ' + hourType + ' hours <button type="button" class="btn btn-danger btn-sm float-end remove-detail-btn">Remove</button></li>';

        // Create hidden input to store the detail for submission
        var hiddenInput = '<input type="hidden" name="TransientDetails" value="' + featureName + ':' + typeName + ':' + hourType + '" id="' + detailId + '-input" />';

        // Append the list item and hidden input to the relevant elements
        //$('#addedDetailsList-transientDetailsModal').append(detailItem);
        //$('#addedDetailsHiddenInputs').append(hiddenInput);
        detailsList.append(detailItem);

        // Attach remove functionality to newly added remove button
        $('.remove-detail-btn').last().on('click', function () {
            $(this).closest('li').remove();
            $('#' + detailId + '-input').remove();
        });

        // Set the hidden inputs for form submission
        $('#hiddenFeatureName').val(featureName);
        $('#hiddenTypeName').val(typeName);
        $('#hiddenHourType').val(hourType);

        // Close the modal
        $('#transientDetailsModal').modal('hide');
    });
});
