document.addEventListener('DOMContentLoaded', function () {
    const radios = document.querySelectorAll('input[type="radio"]');
    const citizenshipSelect = document.getElementById('citizenship-select');
    const specialistBlock = document.getElementById('specialist-block');
    const certificateBlock = document.getElementById('certificate-block');
    const bankExtractBlock = document.getElementById('bank-extract-block');
    const photosBlock = document.getElementById('photos-block');
    const tinBlock = document.getElementById('tin-block');
    const diplomaBlock = document.getElementById('diploma-block');
    const certificateDateBlock = document.getElementById('certificate-date-block');

    const certificateYes = document.getElementById('certificate-yes');
    const certificateNo = document.getElementById('certificate-no');
    const diplomaYes = document.getElementById('diploma-yes');
    const diplomaNo = document.getElementById('diploma-no');
    const certificateDateInput = document.getElementById('certificate-date');

    function resetAllBlocks() {
        certificateBlock.classList.add('hidden');
        bankExtractBlock.classList.add('hidden');
        photosBlock.classList.add('hidden');
        tinBlock.classList.add('hidden');
        diplomaBlock.classList.add('hidden');
        certificateDateBlock.classList.add('hidden');

        [certificateYes, certificateNo, diplomaYes, diplomaNo].forEach(input => {
            input.checked = false;
            input.removeAttribute('required');
        });

        certificateDateInput.value = "";
        certificateDateInput.removeAttribute('required');
        certificateDateInput.disabled = true;

        document.querySelectorAll('#bank-yes, #bank-no, #photos-yes, #photos-no, #tin-yes, #tin-no').forEach(input => {
            input.checked = false;
            input.removeAttribute('required');
        });
    }

    function toggleSpecialistAndQuestions() {
        const isCitizen = ['Азербайджанская Республика', 'Республика Таджикистан',
            'Республика Узбекистан', 'Молдова', 'Украина'].includes(citizenshipSelect.value);

        if (isCitizen) {
            specialistBlock.classList.remove('hidden');
            document.getElementById('specialist-yes').setAttribute('required', 'required');
            document.getElementById('specialist-no').setAttribute('required', 'required');
        } else {
            specialistBlock.classList.add('hidden');
            document.getElementById('specialist-yes').removeAttribute('required');
            document.getElementById('specialist-no').removeAttribute('required');
            document.getElementById('specialist-yes').checked = false;
            document.getElementById('specialist-no').checked = false;

            resetAllBlocks();
        }
    }

    radios.forEach(function (radio) {
        radio.addEventListener('change', function () {
            if (radio.id === 'specialist-no') {
                certificateBlock.classList.remove('hidden');
                bankExtractBlock.classList.remove('hidden');
                photosBlock.classList.remove('hidden');
                tinBlock.classList.remove('hidden');

                document.getElementById('certificate-yes').setAttribute('required', 'required');
                document.getElementById('bank-yes').setAttribute('required', 'required');
                document.getElementById('photos-yes').setAttribute('required', 'required');
                document.getElementById('tin-yes').setAttribute('required', 'required');
            }

            if (radio.id === 'specialist-yes') {
                resetAllBlocks();
            }

            if (radio.id === 'certificate-yes') {
                console.log('попал в certificate-yes');
                certificateDateBlock.classList.remove('hidden');
                certificateDateInput.setAttribute('required', 'required');
                certificateDateInput.disabled = false;
                diplomaBlock.classList.add('hidden');
                diplomaYes.removeAttribute('required');
                diplomaNo.removeAttribute('required');
                diplomaYes.checked = false;
                diplomaNo.checked = false;
            }

            if (radio.id === 'certificate-no') {
                certificateDateBlock.classList.add('hidden');
                certificateDateInput.value = "";
                certificateDateInput.removeAttribute('required');
                certificateDateInput.disabled = true;
                diplomaBlock.classList.remove('hidden');
                diplomaYes.setAttribute('required', 'required');
                diplomaNo.setAttribute('required', 'required');
            }

            if (radio.id === 'diploma-yes') {
                certificateDateBlock.classList.remove('hidden');
                certificateDateInput.setAttribute('required', 'required');
                certificateDateInput.disabled = false;
            }

            if (radio.id === 'diploma-no') {
                certificateDateBlock.classList.add('hidden');
                certificateDateInput.value = "";
                certificateDateInput.removeAttribute('required');
                certificateDateInput.disabled = true;
            }
        });
    });

    citizenshipSelect.addEventListener('change', toggleSpecialistAndQuestions);

    toggleSpecialistAndQuestions();
});