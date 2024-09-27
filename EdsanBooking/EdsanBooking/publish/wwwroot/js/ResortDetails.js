$(document).ready(function () {
    // Resort package details change (only applicable for resort modal)
    $('#packageName').on('change', function () {
        var selectedPackageId = $(this).val();
        $.ajax({
            url: '/api/ResortPackageApi/GetPackageDescription',
            type: 'GET',
            data: { packageId: selectedPackageId },
            success: function (data) {
                $('#packageDescription').val(data.description);
            },
            error: function () {
                $('#packageDescription').val('Error retrieving description');
            }
        });
    });

    // Event listener for adding resort detail to reservation
    $('#addResortDetailToReservation').on('click', function () {
        var packageName = $('#packageName').find('option:selected').text();
        var packageId = $('#packageName').val();
        var packageDescription = $('#packageDescription').val();

        // Create list item and hidden input for added details
        //var detailItem = '<li class="list-group-item">' + packageName + ' - ' + packageDescription + ' <button type="button" class="btn btn-danger btn-sm float-end remove-detail-btn">Remove</button></li>';
        var detailItem = '<li class="list-group-item">' + packageName + ' <button type="button" class="btn btn-danger btn-sm float-end remove-detail-btn">Remove</button></li>';
        var hiddenInput = '<input type="hidden" name="ResortDetails" value="' + packageId + ':' + packageDescription + '" />';

        // Append the list item to the details list
        $('#addedResortDetailsList').append(detailItem);

        // Append the hidden input to a separate container for hidden inputs
        $('#addedDetailsHiddenInputs').append(hiddenInput);

        // Attach remove functionality to newly added remove button
        $('.remove-detail-btn').last().on('click', function () {
            var index = $(this).closest('li').index();
            $(this).closest('li').remove();
            $('#addedDetailsHiddenInputs input:eq(' + index + ')').remove(); // Remove corresponding hidden input
        });

        // Close the modal
        $('#resortDetailsModal').modal('hide');
    });
});
