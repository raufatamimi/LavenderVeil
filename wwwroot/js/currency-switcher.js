/**
 * Currency Switcher Functionality
 */
function initCurrencySwitcher() {
  const currencyLinks = document.querySelectorAll(".top-bar .dropdown-menu a[data-currency]");
  const currentCurrencyDisplay = document.querySelector(".top-bar .dropdown-toggle i.bi-currency-dollar").nextSibling; // Get the text node next to the icon

  // Add JOD and other Arab currencies to the dropdown
  const currencyDropdown = document.querySelector(".top-bar .dropdown-toggle[data-bs-toggle='dropdown'] + .dropdown-menu");
  if (currencyDropdown) {
    // Clear existing items
    currencyDropdown.innerHTML = '';
    
    // Add Arab currencies with JOD as first option
    const currencies = [
      { code: 'JOD', name: 'JOD', symbol: 'د.أ', selected: true },
      { code: 'USD', name: 'USD', symbol: '$' },
      { code: 'SAR', name: 'SAR', symbol: 'ر.س' },
      { code: 'AED', name: 'AED', symbol: 'د.إ' },
      { code: 'EGP', name: 'EGP', symbol: 'ج.م' },
      { code: 'KWD', name: 'KWD', symbol: 'د.ك' },
      { code: 'QAR', name: 'QAR', symbol: 'ر.ق' }
    ];
    
    currencies.forEach(currency => {
      const li = document.createElement('li');
      const a = document.createElement('a');
      a.className = 'dropdown-item';
      a.href = '#';
      a.setAttribute('data-currency', currency.code);
      
      if (currency.selected) {
        const icon = document.createElement('i');
        icon.className = 'bi bi-check2 me-2 selected-icon';
        a.appendChild(icon);
      }
      
      a.appendChild(document.createTextNode(currency.name));
      li.appendChild(a);
      currencyDropdown.appendChild(li);
    });
    
    // Update the displayed currency in the dropdown toggle to JOD
    if (currentCurrencyDisplay && currentCurrencyDisplay.nodeType === Node.TEXT_NODE) {
      currentCurrencyDisplay.textContent = ' JOD';
    }
  }

  // Re-select currency links after dynamically adding them
  const updatedCurrencyLinks = document.querySelectorAll(".top-bar .dropdown-menu a[data-currency]");
  
  updatedCurrencyLinks.forEach(link => {
    link.addEventListener("click", function(e) {
      e.preventDefault();
      const selectedCurrency = this.getAttribute("data-currency");
      const selectedCurrencyText = this.textContent.trim();

      // Update the displayed currency in the dropdown toggle
      if (currentCurrencyDisplay && currentCurrencyDisplay.nodeType === Node.TEXT_NODE) {
        currentCurrencyDisplay.textContent = ` ${selectedCurrencyText}`;
      }

      // Update selected state in dropdown
      updatedCurrencyLinks.forEach(l => l.querySelector(".selected-icon")?.remove()); // Remove existing checkmark
      const checkIcon = document.createElement("i");
      checkIcon.className = "bi bi-check2 me-2 selected-icon";
      this.prepend(checkIcon);

      // Update currency symbol and prices throughout the site
      updateCurrencyDisplay(selectedCurrency);
      
      console.log(`Currency switched to: ${selectedCurrency}`);
    });
  });

  // Function to update currency symbols and convert prices
  function updateCurrencyDisplay(currencyCode) {
    // Exchange rates (approximate, as of May 2025)
    const exchangeRates = {
      'USD': 1,
      'JOD': 0.71,  // 1 USD = 0.71 JOD
      'SAR': 3.75,  // 1 USD = 3.75 SAR
      'AED': 3.67,  // 1 USD = 3.67 AED
      'EGP': 30.90, // 1 USD = 30.90 EGP
      'KWD': 0.31,  // 1 USD = 0.31 KWD
      'QAR': 3.64   // 1 USD = 3.64 QAR
    };
    
    // Currency symbols
    const currencySymbols = {
      'USD': '$',
      'JOD': 'د.أ',
      'SAR': 'ر.س',
      'AED': 'د.إ',
      'EGP': 'ج.م',
      'KWD': 'د.ك',
      'QAR': 'ر.ق'
    };
    
    // Get all price elements
    const priceElements = document.querySelectorAll('.price, .product-price, .cart-total-price');
    
    priceElements.forEach(element => {
      // Extract the numeric value from the price text
      const priceText = element.textContent;
      const numericValue = parseFloat(priceText.replace(/[^0-9.]/g, ''));
      
      if (!isNaN(numericValue)) {
        // Convert from USD to selected currency
        const convertedValue = (numericValue / exchangeRates['USD']) * exchangeRates[currencyCode];
        
        // Format the price with the new currency symbol
        const symbol = currencySymbols[currencyCode];
        const formattedPrice = currencyCode === 'USD' 
          ? `$${convertedValue.toFixed(2)}` 
          : `${convertedValue.toFixed(2)} ${symbol}`;
        
        // Update the element text
        element.textContent = formattedPrice;
      }
    });
  }

  // Initialize with JOD by default
  updateCurrencyDisplay('JOD');
}

// Add currency switcher initialization to existing load/ready listeners
window.addEventListener("load", initCurrencySwitcher);
