var element = document.getElementById('element-to-print');
html2pdf(element, {
    margin: 1,
    filename: 'podaci.pdf',
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { dpi: 192, letterRendering: true },
    jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
});    