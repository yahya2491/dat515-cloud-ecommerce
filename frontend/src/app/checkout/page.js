"use client";

import { useEffect, useState, useContext } from "react";
import { useRouter } from "next/navigation";
import { UserContext } from "../../context/UserContext";
import {
  getUserCart,
  checkoutPayment,
  getAllProducts,
  clearCart, // optional backend call if you add it later
} from "../../utils/requests";
import { recommendProducts } from "../../utils/recommendations";
import styles from "../page.module.css";

export default function CheckoutPage() {
  const { user, initialized } = useContext(UserContext);
  const router = useRouter();

  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const [checkoutMessage, setCheckoutMessage] = useState("");
  const [email, setEmail] = useState("");

  // Dummy card data
  const [cardName, setCardName] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [expiry, setExpiry] = useState("");
  const [cvv, setCvv] = useState("");

  // Recommendations
  const [recommendations, setRecommendations] = useState([]);

  useEffect(() => {
    if (!initialized || !user?.id) return;

    async function fetchCartAndRecommendations() {
      try {
        const data = await getUserCart(user.id);
        setCart(data);
        setEmail(`${user.username}@example.com`);

        // Fetch recommendations if cart has items
        if (data.items?.length > 0) {
          const allProducts = await getAllProducts();
          const firstProduct = data.items[0].product || data.items[0];
          const recs = recommendProducts(firstProduct, allProducts);
          setRecommendations(recs);
        }
      } catch (err) {
        console.error(err);
        alert("Failed to load cart");
        router.push("/cart");
      } finally {
        setLoading(false);
      }
    }

    fetchCartAndRecommendations();
  }, [initialized, user, router]);

  async function handleCheckout() {
    if (!cart || !cart.items?.length) {
      alert("Your cart is empty!");
      return;
    }

    if (!cardName || !cardNumber || !expiry || !cvv) {
      alert("Please fill out all card fields.");
      return;
    }

    setCheckoutLoading(true);
    setCheckoutMessage("");

    try {
      const paymentRequest = {
        amount: Math.round(cart.totalPrice * 100),
        currency: "usd",
        description: `Payment for cart ${cart.id}`,
        receiptEmail: `${user.username}@example.com`,
      };

      console.log("🔹 Sending payment request:", paymentRequest);

      const result = await checkoutPayment(user.id, paymentRequest);

      console.log("✅ Payment result:", result);

      setCheckoutMessage(
  `✅ Payment successful! Payment ID: ${result.paymentId} — Total Paid: $${cart.totalPrice.toFixed(2)}`
);


      // Clear the cart after successful payment
      
  // Payment successful, now clear cart
  await clearCart(cart.id);
  setCart(null); // empty cart in frontend


      // Auto redirect after 5 seconds
      setTimeout(() => router.push("/"), 5000);
    } catch (err) {
      console.error("❌ Payment error:", err);
      setCheckoutMessage(`❌ Payment failed: ${err.message}`);
    } finally {
      setCheckoutLoading(false);
    }
  }

  if (!initialized || loading)
    return <p style={{ textAlign: "center", marginTop: "2rem" }}>Loading...</p>;

  // ✅ Confirmation screen
  if (checkoutMessage.startsWith("✅")) {
    return (
      <main className={styles.checkoutPage}>
        <div className={styles.confirmationBox}>
          <h1>🎉 Payment Successful!</h1>
          <p>
            Thank you for your purchase, <strong>{user.nickname}</strong>!
          </p>
          <p>Your payment has been processed successfully.</p>
          <p>
            <strong>{checkoutMessage}</strong>
          </p>
          <p>Redirecting to home in 5 seconds...</p>
          <button
            onClick={() => router.push("/")}
            className={styles.checkoutButton}
          >
            Continue Shopping 🛍️
          </button>
        </div>
      </main>
    );
  }

  return (
    <main className={styles.checkoutPage}>
      <h1>💳 Checkout</h1>

      {cart && cart.items?.length > 0 ? (
        <>
          <ul className={styles.cartList}>
            {cart.items.map((item) => (
              <li key={item.productId} className={styles.cartItem}>
                <strong>{item.productName}</strong> — {item.quantity} × ${item.unitPrice.toFixed(2)}
                <span style={{ marginLeft: "1rem" }}>
                  = ${(item.unitPrice * item.quantity).toFixed(2)}
                </span>
              </li>
            ))}
          </ul>

          <h3>Total: ${cart.totalPrice.toFixed(2)}</h3>

          {recommendations.length > 0 && (
            <div className={styles.recommendationsSection}>
              <h3>You might also like:</h3>
              <div className={styles.recommendationsGrid}>
                {recommendations.map((prod) => (
                  <div key={prod.id} className={styles.recommendationCard}>
                    <p><strong>{prod.name}</strong></p>
                    <p>${prod.price.toFixed(2)}</p>
                  </div>
                ))}
              </div>
            </div>
          )}

          <div style={{ marginTop: "1rem" }}>
            <label>
              Receipt Email:
              <input
                type="email"
                value={email}
                readOnly
                style={{ marginLeft: "0.5rem", padding: "0.25rem", width: "250px" }}
              />
            </label>
          </div>

          <div className={styles.cardForm}>
            <h3>Card Information</h3>
            <input
              type="text"
              placeholder="Cardholder Name"
              value={cardName}
              onChange={(e) => setCardName(e.target.value)}
              className={styles.cardInput}
            />
            <input
              type="text"
              placeholder="Card Number"
              maxLength="16"
              value={cardNumber}
              onChange={(e) => setCardNumber(e.target.value.replace(/\D/g, ""))}
              className={styles.cardInput}
            />
            <div className={styles.cardRow}>
              <input
                type="text"
                placeholder="MM/YY"
                maxLength="5"
                value={expiry}
                onChange={(e) => setExpiry(e.target.value)}
                className={styles.cardInput}
              />
              <input
                type="text"
                placeholder="CVV"
                maxLength="3"
                value={cvv}
                onChange={(e) => setCvv(e.target.value.replace(/\D/g, ""))}
                className={styles.cardInput}
              />
            </div>
          </div>

          <button
            onClick={handleCheckout}
            disabled={checkoutLoading}
            className={styles.checkoutButton}
          >
            {checkoutLoading ? "Processing..." : "Pay Now"}
          </button>

          {checkoutMessage && (
            <p
              className={`${styles.checkoutMessage} ${
                checkoutMessage.startsWith("✅") ? styles.success : styles.error
              }`}
            >
              {checkoutMessage}
            </p>
          )}
        </>
      ) : (
        <p>Your cart is empty.</p>
      )}
    </main>
  );
}