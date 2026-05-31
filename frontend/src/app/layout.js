import "./globals.css";
import styles from "./page.module.css";
import Navbar from "../components/Navbar"; // import the new Navbar
import { UserProvider } from "../context/UserContext";

export const metadata = {
  title: "FreshMart - Online Grocery",
  description: "Shop fresh groceries online",
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body className={styles.body}>
        <UserProvider>
        {/*  Navbar */}
        <Navbar />

        {/* Page content */}
        <div className={styles.pageWrapper}>{children}</div>
        </UserProvider>
      </body>
    </html>
  );
}
