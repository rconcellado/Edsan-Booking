$(document).ready(function () {
    // Show correct modal based on Reservation Type
    $('#addPackageDetailsButton').on('click', function () {
        var type = $('#ReservationType').val();
        if (type === 'Transient') {
            $('#transientDetailsModal').modal('show');
        } else if (type === 'Resort') {
            $('#resortDetailsModal').modal('show');
        } else {
            alert('Please select a valid reservation type.');
        }
    });
});
